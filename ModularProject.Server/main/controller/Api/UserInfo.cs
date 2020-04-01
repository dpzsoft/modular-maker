using System;
using dpz3;
using dpz3.Modular;

namespace control {

    [Modular(ModularTypes.SessionApi, "/Api/{ControllerName}")]
    public class UserInfo : JttpSessionControllerBase {

        [Modular(ModularTypes.Post, "Logout")]
        public IResult Logout() {
            // 存储登录信息
            Session.SetValue("UserID", "");
            return Success();
        }
    }
}
