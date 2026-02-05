using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public static class TextureCache
    {
        public const bool UseSmallImg = false; // For faster debug
        
        private static readonly Dictionary<string, Sprite> _spriteCache = new();

        private static readonly string ArtFolderName = UseSmallImg ? "../GameDataLight/" : "../GameData/";
        private static readonly string ArtRoot = Path.Combine(Application.dataPath, ArtFolderName);

        private static bool _isPreloading = false;
        
        public static async Task PreloadWithProgress(Slider progressBar = null, Text progressText = null)
        {
            progressBar?.gameObject.SetActive(true);
            progressText?.gameObject.SetActive(true);

            if (progressBar != null && progressText != null)
            {
                progressBar.value = 0f;
                if (progressText) progressText.text = "Loading textures...";

                await PreloadAllAsync(
                    progress =>
                    {
                        progressBar.value = progress;
                        if (progressText) progressText.text = $"Loading textures... {(progress * 100f):F0}%";
                    },
                    () =>
                    {
                        progressBar.value = 1f;
                        if (progressText) progressText.text = "Loading textures complete";
                    }
                );
            }
        }
        
        public static async Task PreloadAllAsync(System.Action<float> onProgress = null, System.Action onComplete = null)
        {
            if (_isPreloading)
            {
                Debug.LogWarning("[TextureCache] Already preloading.");
                return;
            }

            _isPreloading = true;

            if (!Directory.Exists(ArtRoot))
            {
                Debug.LogError($"[TextureCache] Art root not found: {ArtRoot}");
                _isPreloading = false;
                return;
            }

            var files = Directory.GetFiles(ArtRoot, "*.*", SearchOption.AllDirectories);
            var imageFiles = (from file in files let ext = Path.GetExtension(file).ToLower() where ext is ".png" or ".jpg" or ".jpeg" or ".webp" select file).ToList();

            var total = imageFiles.Count;
            var loaded = 0;

            Debug.Log($"[TextureCache] Async preloading {total} textures...");

            foreach (var file in imageFiles)
            {
                var key = GetRelativeKey(file);
                if (_spriteCache.ContainsKey(key))
                {
                    loaded++;
                    continue;
                }

                var bytes = await File.ReadAllBytesAsync(file);
                if (bytes == null || bytes.Length == 0)
                {
                    Debug.LogWarning($"[TextureCache] Empty file: {file}");
                    continue;
                }

                var tex = new Texture2D(2, 2);
                tex.LoadImage(bytes);
                var sprite = Sprite.Create(tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f));
                _spriteCache[key] = sprite;

                loaded++;
                onProgress?.Invoke((float)loaded / total);

                if (loaded % 5 == 0)
                    await Task.Yield();
            }

            _isPreloading = false;
            Debug.Log($"[TextureCache] Preload complete: {loaded}/{total}");
            onProgress?.Invoke(1f);
            onComplete?.Invoke();
        }

        public static Sprite GetSprite(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            var key = NormalizeKey(relativePath);
            if (_spriteCache.TryGetValue(key, out var sprite))
                return sprite;

            var fullPath = Path.Combine(ArtRoot, key + ".png");
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"[TextureCache] Missing file: {fullPath}");
                return null;
            }

            var bytes = File.ReadAllBytes(fullPath);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            sprite = Sprite.Create(tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f));
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static void Clear()
        {
            _spriteCache.Clear();
        }

        private static string GetRelativeKey(string fullPath)
        {
            var relative = Path.GetRelativePath(ArtRoot, fullPath);
            relative = relative.Replace("\\", "/");
            return Path.ChangeExtension(relative, null);
        }

        private static string NormalizeKey(string relativePath)
        {
            return relativePath.Replace("\\", "/").Trim();
        }
    }
}