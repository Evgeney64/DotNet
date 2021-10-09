using System;
using System.Collections.Generic;
using System.Text;

namespace Hcs.Stores.EFCore
{
    class Class1
    {
        public ServiceResult Update_ItemExts<TExt>(
            string sender,
            DbSet<TExt> object_ext_Set,
            List<TExt> _object_ext_items,
            long Pk,
            string prop_FK_name_ID,
            string mess,
            Func<IEntityObject, RegimEdit, List<Tuple<string, string, string>>, ServiceResult> calcParam_SetConsolidation = null,
            bool Update_T = true    // _object_item - уже взято из this.ObjectContext
            )
            where TExt : class, IEntityObject
        {
            #region

            #region Get object_ext_List
            PropertyInfo prop_Fk = typeof(TExt).GetProperty(prop_FK_name_ID);
            //PropertyInfo prop_Pk = typeof(T).GetProperty(prop_FK_name_ID);
            //long Pk = (long)prop_Pk.GetValue(_object_item, null);

            Expression<Func<TExt, bool>> expr_where_TExt_eq_Fk = Tsb.WCF.Web.Public.GetExpr_WhereId<TExt>(prop_Fk, Pk);
            // ss => (ss.FACILITY_PRODUCT_ID == 2020100000000466030)

            IQueryable<TExt> object_ext_List = object_ext_Set.Where(expr_where_TExt_eq_Fk);
            if (object_ext_List == null)
                return Tsb.WCF.Web.Public.ServiceResult_SetError("Не найден объект TExt");
            #endregion

            #region ss
            ParameterExpression qry_prm_ss = Expression.Parameter(typeof(TExt), "ss");
            //Expression<Func<TExt, DateTime>> expr_ByDateBeg = Expression.Lambda<Func<TExt, DateTime>>(
            //    Expression.Property(qry_prm_ss, "DATE_BEG"),
            //    qry_prm_ss
            //    );
            // ss => (ss.DATE_BEG)
            #endregion

            #region TExt (редактирование или добавление новых строк)
            //foreach (TExt _item_ext in object_ext_Set.OrderBy(expr_ByDateBeg))
            if (_object_ext_items != null)
            {
                #region
                foreach (TExt _item_ext in _object_ext_items)
                {
                    DateTime value_DateBeg = (DateTime)typeof(TExt).GetProperty("DATE_BEG").GetValue(_item_ext, null);
                    //ParameterExpression qry_prm_ss = Expression.Parameter(typeof(TExt), "ss");

                    Expression expr_DateBeg = Expression.MakeBinary(
                        ExpressionType.Equal,
                        Expression.Property(qry_prm_ss, "DATE_BEG"),
                        Expression.Constant(value_DateBeg, typeof(DateTime))
                        );
                    // (ss.DATE_BEG == 11.11.2013 0:00:00)

                    Expression<Func<TExt, bool>> expr_where_DateBeg = (Expression<Func<TExt, bool>>)Expression.Lambda(
                        typeof(Func<TExt, bool>),
                        expr_DateBeg,
                        new ParameterExpression[] { qry_prm_ss }
                        );
                    // ss => (ss.DATE_BEG == 11.11.2013 0:00:00)

                    TExt item_ext = object_ext_List.Where(expr_where_DateBeg).FirstOrDefault();
                    if (item_ext != null)
                    {
                        // запись на дату существует => меняем свойства
                        List<Tuple<string, string, string>> diff = new List<Tuple<string, string, string>>();
                        Tsb.WCF.Web.Public.CopyProperties(item_ext, _item_ext, null, diff);

                        if (calcParam_SetConsolidation != null)
                            calcParam_SetConsolidation(_item_ext, RegimEdit.Update, diff);
                    }
                    else
                    {
                        if (calcParam_SetConsolidation != null)
                            calcParam_SetConsolidation(_item_ext, RegimEdit.Insert, null);

                        // запись на дату не существует => добавляем запись
                        object_ext_Set.Add(_item_ext);
                    }
                }
                #endregion
            }
            #endregion

            #region TExt (удаление строк)
            // foreach (TExt _item_ext in object_ext_List.Where(expr_EqId).OrderBy(expr_ByDateBeg))
            if (_object_ext_items != null)
            {
                foreach (TExt _item_ext in object_ext_List)
                {
                    DateTime value_DateBeg = (DateTime)typeof(TExt).GetProperty("DATE_BEG").GetValue(_item_ext, null);
                    bool date_exists = false;
                    foreach (TExt _item_ext1 in _object_ext_items)
                    {
                        DateTime value_DateBeg1 = (DateTime)typeof(TExt).GetProperty("DATE_BEG").GetValue(_item_ext1, null);
                        if (value_DateBeg1 == value_DateBeg)
                        {
                            date_exists = true;
                            break;
                        }
                    }
                    if (date_exists == false)
                    {
                        if (calcParam_SetConsolidation != null)
                            calcParam_SetConsolidation(_item_ext, RegimEdit.Delete, null);

                        //MethodInfo method = object_ext_Set.GetType().GetMethod("DeleteObject");
                        //if (method == null) return Public.ServiceResult_SetError("Не найден метод DeleteObject");

                        //method.Invoke(object_ext_Set, new object[] { _item_ext });
                        object_ext_Set.Remove(_item_ext);
                    }
                }
            }
            #endregion

            #region try / catch
            try
            {
                this.Context.SaveChanges(); //фактическое сохранение в БД

                //DoLog(_object_item, SysOperation_Enum.Edit, diff, sender); //журнал
            }
            catch (Exception e)
            {
                return Tsb.WCF.Web.Public.ServiceResult_SetError(mess, e);
            }
            #endregion

            #region result
            ServiceResult result =
                new ServiceResult
                {
                };

            return result;
            #endregion
            #endregion
        }
    }
}
