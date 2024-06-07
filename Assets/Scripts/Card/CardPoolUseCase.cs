using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Card
{
    /**
     * 卡牌池，所有卡牌示例从卡牌池拿。
     */
    public class CardPoolUseCase
    {
        private Dictionary<string, BaseCardEntity> _allCardPool;

        public CardPoolUseCase()
        {
            // 初始化时,会从Json加载所有卡牌实体类。
            var entity = JsonConvert.DeserializeObject<List<BaseCardEntity>>("");
            this._allCardPool = entity.ToDictionary(it => it.id);
        }

        public CardInstance GetCardInstanceById(String id)
        {
            // 重新new一个卡牌示例。
            return _allCardPool[id].newInstance();
        }
    }
}