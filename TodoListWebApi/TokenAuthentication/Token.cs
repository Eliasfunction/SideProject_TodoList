using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListApi.TokenAuthentication
{
    public class Token
    {
        //令牌值
        public string TokenValue { get; set; }
        //刷新令牌值
        public string RefreshTokenValue { get; set; }
        //到期時間
        public DateTime ExpiryDate { get; set; }

    }
}
