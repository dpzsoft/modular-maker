using System;
using dpz3;
using dpz3.Modular;

namespace control {

    [Modular(ModularTypes.SessionApi, "/Api/{ControllerName}")]
    public class Session : JttpSessionControllerBase {

        [Modular(ModularTypes.Post, "Create")]
        public IResult Create() {
            string sid = Session.CreateSessionId();
            string key = Guid.NewGuid().ToString().Replace("-", "");
            // 存储安全密钥
            Session.SetValue("SecurityKey", key);
            // 返回信息
            Response.Data
                .String("SessionID", sid)
                .String("SecurityKey", key)
                .String("SecurityTime", $"{dpz3.Time.Now.ToMillisecondsTimeStamp()}");
            return Success();
        }

        [Modular(ModularTypes.Post, "CreateSecurityKey")]
        public IResult CreateSecurityKey() {
            // 获取现有的安全密钥
            string key = Session.GetValue("SecurityKey");
            if (!key.IsNoneOrNull()) return Fail("安全密钥已经存在，无法重复申请。");
            // 生成一个新的安全密钥
            key = Guid.NewGuid().ToString().Replace("-", "");
            // 存储安全密钥
            Session.SetValue("SecurityKey", key);
            // 返回信息
            Response.Data
                .String("SecurityKey", key);
            return Success();
        }

        [Modular(ModularTypes.Post, "Keep")]
        public IResult Keep() {
            Response.Data.Number("Enable", Session.Enable ? 1 : 0);
            return Success();
        }

    }
}
