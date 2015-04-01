using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;
using NetEntityHWQ;

public static class ExpandEntity
{
    public static UserRole ChangeToUserRole(this RpgUserRoleT rut)
    {
        return new UserRole()
        {
            exp = rut.RoleExp,
            rigureId = rut.RoleIdConfig,
            roleId = rut.RoleId,
            roleLevel = rut.RoleLevel,
            roleName = rut.RoleName,
            userId = rut.UserId,
            pointX = rut.LastPointX,
            pointY = rut.LastPointY,
            pointZ = rut.LastPointZ
        };
    }

    public static QueryUserResult ChangeToQueryUserResult(this RpgUserTableT r)
    {
        QueryUserResult qur = new QueryUserResult();
        qur.enable = r.Enable;
        qur.userID = r.UserId;
        qur.name = r.UserName;
        qur.pwd = r.Passwrod;
        return qur;
    }


    public static QueryUserResult ChangeToQueryUserResult(this RpgMapData r)
    {
        QueryUserResult qur = new QueryUserResult();
        return qur;
    }
}

