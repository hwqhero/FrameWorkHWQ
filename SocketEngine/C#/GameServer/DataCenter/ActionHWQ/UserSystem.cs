using Entity;
using MySql.Data.MySqlClient;
using NetEntityHWQ;
using ServerEngine.OperationObject;
using ServerEngine.ServerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCenter.ActionHWQ
{
    class UserSystem : BaseSystem
    {
        [SystemCMDAttr(1, 1, "查询用户")]
        private void FindUser(OperationData od)
        {
            QueryUser qu = od.NetObject<QueryUser>();
            QueryUserResult qur = null;
   
            DataCenterMain.Insance.OperationTable<RpgUserTableT>(list =>
            {

                RpgUserTableT rut = list.Find(obj => obj.UserName.Equals(qu.userName));
                if (rut != null)
                {
                    qur = rut.ChangeToQueryUserResult();
                }
                else
                {
                    qur = new QueryUserResult();
                }
            });
            qur.clientID = qu.clientID;
            od.User.SendData(1, 1, qur);
        }

        [SystemCMDAttr(1, 2, "查询用户")]
        private void FindUser2(OperationData od)
        {
            QueryUser qu = od.NetObject<QueryUser>();
            QueryUserResult qur = null;
        
            DataCenterMain.Insance.OperationTable<RpgUserTableT>(list =>
            {
                RpgUserTableT rut = list.Find(obj => obj.UserName.Equals(qu.userName));
                if (rut == null)
                {
                    string sql = "insert into rpg_user_table_t values(null,?a,?b,1)";
                    long id = DataCenterMain.Insance.Inster(sql, new MySqlParameter("?a", qu.userName), new MySqlParameter("?b", qu.pwd));

                    RpgUserTableT rut1 = DataCenterMain.Insance.GetObjectBySql<RpgUserTableT>("where user_id = ?id", new MySqlParameter("?id", id));
                    qur = rut1.ChangeToQueryUserResult();
                    id = DataCenterMain.Insance.Inster("insert into rpg_user_t values(?a,0,50)", new MySqlParameter("?a", id));
                    DataCenterMain.Insance.GetObjectBySql<RpgUserT>("where user_id = ?id", new MySqlParameter("?id", id));
                }
                else
                {
                    qur = new QueryUserResult();
                    qur.errorCode = 3;
                }
            });
            qur.clientID = qu.clientID;
            od.User.SendData(1, 2, qur);
        }

        [SystemCMDAttr(1, 3, "查询角色")]
        private void FindRole(OperationData od)
        {
            RoleResult rr = od.NetObject<RoleResult>();
            DataCenterMain.Insance.OperationTable<RpgUserRoleT>(list =>
            {
                rr.roleList = new List<UserRole>();
                foreach (RpgUserRoleT rurt in list.FindAll(obj => obj.UserId == rr.userId))
                {
                    rr.roleList.Add(rurt.ChangeToUserRole());
                }
                od.User.SendData(1, 3, rr);
            });
        }

        [SystemCMDAttr(1, 4, "创建角色")]
        private void CreateRole(OperationData od)
        {
            CreateRoleData crd = od.NetObject<CreateRoleData>();
            RoleResult rr = new RoleResult();
            rr.code = crd.code;
            
            RpgUserRoleT rut = DataCenterMain.Insance.FindByTable<RpgUserRoleT>(obj => string.Equals(obj.RoleName,crd.roleName));
            if (rut == null)
            {
                RpgMapT map = DataCenterMain.Insance.FindByTable<RpgMapT>(obj => obj.MapId == 1);
                string sql = "insert into rpg_user_role_t values(null,?userId,1,?rolename,0,?fid,?x,?y,?z,1)";
                long id = DataCenterMain.Insance.Inster(sql,
                    new MySqlParameter("?x", map.MapRebornX),
                    new MySqlParameter("?y", map.MapRebornY),
                    new MySqlParameter("?z", map.MapRebornZ),
                    new MySqlParameter("?userId", crd.userId),
                    new MySqlParameter("?rolename", crd.roleName),
                    new MySqlParameter("?fid", crd.fid)
                    );
                rr.roleList = new List<UserRole>();
                rut = DataCenterMain.Insance.GetObjectBySql<RpgUserRoleT>("where role_id = ?role_id", new MySqlParameter("?role_id", id));
                rr.roleList.Add(rut.ChangeToUserRole());
            }
            else
            {
                rr.errorCode = 3;//角色名重复    
            }
            
            od.User.SendData(1, 4, rr);
        }
    }
}

