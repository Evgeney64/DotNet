using Hcs.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace Hcs
{
    public class Relation
    {
        public string ReferenceKey { get; set; }
        public string ReferenceProperty { get; set; }
        public IEntityRelation Reference { get; set; }
    }

    public interface IEntityRelation
    {
        Type Type { get; }
        IEnumerable<KeyValuePair<string, Relation>> Navigation { get; }
    }

    public partial class EntityRelationBuilder
    {
        private readonly IDictionary<Type, IEntityRelation> entities = new Dictionary<Type, IEntityRelation>();
        
        public EntityRelationBuilder()
        {
            EntityRelationBuilder entityRelationBuilder = this;

            #region Nsi
            entityRelationBuilder
                .Entity<NsiExportRequest>()
                .Entity<NsiExportResult>(e =>
                {
                    e.Navigate(a => a.NsiExportResultFields, ss => ss.NsiExportTransportGUID, ss => ss.NsiExportTransportGU);
                });
            #endregion

            #region Attachment
            entityRelationBuilder
                .Entity<AttachmentPostRequest>()
                .Entity<AttachmentPostResult>();
            #endregion

            #region Organization
            entityRelationBuilder
                .Entity<OrganizationExportRequest>(e =>
                {
                    e.Navigate(s => s.OrganizationExportRequestData, ss => ss.OrganizationExportTransportGUID, ss => ss.OrganizationExportTransportGU);
                })
                .Entity<OrganizationExportResult>(e =>
                {
                    e.Navigate(s => s.OrganizationExportResultLegal, ss => ss.TransportGUID, ss => ss.TransportGU);
                    e.Navigate(s => s.OrganizationExportResultEntp, ss => ss.TransportGUID, ss => ss.TransportGU);
                    e.Navigate(s => s.OrganizationExportResultRoles, ss => ss.OrganizationExportTransportGUID, ss => ss.OrganizationExportTransportGU);
                });
            #endregion

            #region Account
            entityRelationBuilder
                .Entity<AccountImportRequest>(e =>
                {
                    e.Navigate(a => a.AccountImportRequestReasons, ss => ss.AccountImportTransportGUID, ss => ss.AccountImportTransportGU);
                    e.Navigate(a => a.AccountImportRequestPayer, ss => ss.TransportGUID, ss => ss.TransportGU);
                    e.Navigate(a => a.AccountImportRequestPercentPremises, ss => ss.AccountImportTransportGUID, ss => ss.AccountImportTransportGU);
                })
                .Entity<AccountImportResult>(e =>
                {
                    e.Navigate(a => a.AccountImportResultErrors, ss => ss.AccountImportTransportGUID, ss => ss.AccountImportTransportGU);
                })
                .Entity<AccountExportRequest>()
                .Entity<AccountExportResult>(e =>
                {
                    e.Navigate(a => a.AccountExportResultPercentPremises, ss => ss.AccountExportTransportGUID, ss => ss.AccountExportTransportGU);
                });
            #endregion

            #region Ack
            entityRelationBuilder
                .Entity<AckImportRequest>()
                .Entity<AckImportCancellationRequest>()
                .Entity<AckImportResult>(e =>
                {
                    e.Navigate(a => a.AckImportResultErrors, ss => ss.AckImportTransportGUID, ss => ss.AckImportTransportGU);
                });
            #endregion

            #region Contract
            entityRelationBuilder
                .Entity<ContractImportRequest>(e =>
                {
                    var objAddr = e.Navigate(c => c.ContractImportRequestObjectAddresses, ss => ss.ContractImportTransportGUID, ss => ss.ContractImportTransportGU);
                    objAddr.Navigate(o => o.ContractImportRequestObjectServiceResources, ss => ss.ContractImportObjectAddressTransportGUID, ss => ss.ContractImportObjectAddressTransportGU);
                    objAddr.Navigate(o => o.ContractImportRequestSubjectQualityIndicators, ss => ss.ContractImportObjectAddressTransportGUID, ss => ss.ContractImportObjectAddressTransportGU);
                    var subj = e.Navigate(c => c.ContractImportRequestSubjects, ss => ss.ContractImportTransportGUID, ss => ss.ContractImportTransportGU);
                    subj.Navigate(s => s.ContractImportRequestSubjectQualityIndicators, ss => ss.ContractImportSubjectTransportGUID, ss => ss.ContractImportSubjectTransportGU);
                    subj.Navigate(s => s.ContractImportRequestObjectServiceResources, ss => ss.ContractImportSubjectTransportGUID, ss => ss.ContractImportSubjectTransportGU);
                    e.Navigate(c => c.ContractImportRequestParty, ss => ss.TransportGUID, ss => ss.TransportGU);
                    e.Navigate(c => c.ContractImportRequestAttachments, ss => ss.ContractImportTransportGUID, ss => ss.ContractImportTransportGU);
                })
                .Entity<ContractImportResult>(e =>
                {
                    e.Navigate(c => c.ContractImportResultErrors, ss => ss.ContractImportTransportGUID, ss => ss.ContractImportTransportGU);
                });
            #endregion

            #region Settlement
            entityRelationBuilder
                .Entity<SettlementImportRequest>(e =>
                {
                    var per = e.Navigate(s => s.SettlementImportRequestPeriods, ss => ss.SettlementImportTransportGUID, ss => ss.SettlementImportTransportGU);
                    per.Navigate(s => s.SettlementImportRequestPeriodInfo, ss => ss.TransportGUID, ss => ss.TransportGU);
                    per.Navigate(s => s.SettlementImportRequestPeriodAnnulment, ss => ss.TransportGUID, ss => ss.TransportGU);
                })
                .Entity<SettlementImportAnnulmentRequest>()
                .Entity<SettlementImportResult>(e =>
                {
                    e.Navigate(a => a.SettlementImportResultErrors, ss => ss.SettlementImportTransportGUID, ss => ss.SettlementImportTransportGU);
                });
            #endregion

            #region Device
            entityRelationBuilder
                .Entity<DeviceImportRequest>(e =>
                {
                    e.Navigate(c => c.DeviceImportRequestAccounts, ss => ss.DeviceImportTransportGUID, ss => ss.DeviceImportTransportGU);
                    e.Navigate(c => c.DeviceImportRequestAddresses, ss => ss.DeviceImportTransportGUID, ss => ss.DeviceImportTransportGU);
                    e.Navigate(c => c.DeviceImportRequestLinkedDevices, ss => ss.DeviceImportTransportGUID, ss => ss.DeviceImportTransportGU);
                    e.Navigate(c => c.DeviceImportRequestValues, ss => ss.DeviceImportTransportGUID, ss => ss.DeviceImportTransportGU);
                })
                .Entity<DeviceImportArchiveRequest>()
                .Entity<DeviceImportReplaceRequest>(e =>
                {
                    e.Navigate(c => c.DeviceImportReplaceRequestValues, ss => ss.DeviceImportReplaceTransportGUID, ss => ss.DeviceImportReplaceTransportGU);
                })
                .Entity<DeviceImportResult>(e =>
                {
                    e.Navigate(c => c.DeviceImportResultErrors, ss => ss.DeviceImportTransportGUID, ss => ss.DeviceImportTransportGU);
                });
            #endregion

            #region DeviceValue
            entityRelationBuilder
                .Entity<DeviceValueImportRequest>()
                .Entity<DeviceValueImportResult>(e =>
                {
                    e.Navigate(c => c.DeviceValueImportResultErrors, ss => ss.DeviceValueImportTransportGUID, ss => ss.DeviceValueImportTransportGU);
                })
                .Entity<DeviceValueExportRequest>(e =>
                {
                    e.Navigate(c => c.DeviceValueExportRequestDevices, ss => ss.DeviceValueExportTransportGUID, ss => ss.DeviceValueExportTransportGU);
                    e.Navigate(c => c.DeviceValueExportRequestDeviceTypes, ss => ss.DeviceValueExportTransportGUID, ss => ss.DeviceValueExportTransportGU);
                    e.Navigate(c => c.DeviceValueExportRequestMunicipalResources, ss => ss.DeviceValueExportTransportGUID, ss => ss.DeviceValueExportTransportGU);
                })
                .Entity<DeviceValueExportResult>();
            #endregion

            #region House
            entityRelationBuilder
                .Entity<HouseImportRequest>(e =>
                {
                    e.Navigate(c => c.HouseImportRequestBlocks, ss => ss.HouseImportTransportGUID, ss => ss.HouseImportTransportGU)
                        .Navigate(ss => ss.HouseImportRequestLivingRooms, ss => ss.HouseImportBlockTransportGUID, ss => ss.HouseImportBlockTransportGU);
                    e.Navigate(c => c.HouseImportRequestEntrances, ss => ss.HouseImportTransportGUID, ss => ss.HouseImportTransportGU)
                        .Navigate(ss => ss.HouseImportRequestPremises, ss => ss.HouseImportEntranceTransportGUID, ss => ss.HouseImportEntranceTransportGU);
                    e.Navigate(c => c.HouseImportRequestPremises, ss => ss.HouseImportTransportGUID, ss => ss.HouseImportTransportGU)
                        .Navigate(ss => ss.HouseImportRequestLivingRooms, ss => ss.HouseImportPremiseTransportGUID, ss => ss.HouseImportPremiseTransportGU);
                    e.Navigate(c => c.HouseImportRequestLivingRooms, ss => ss.HouseImportTransportGUID, ss => ss.HouseImportTransportGU);
                })
                .Entity<HouseImportResult>(e =>
                {
                    e.Navigate(c => c.HouseImportResultErrors, ss => ss.HouseImportTransportGUID, ss => ss.HouseImportTransportGU);
                    e.Navigate(c => c.HouseImportResultBlocks, ss => ss.HouseImportTransportGUID, ss => ss.HouseImportTransportGU)
                        .Navigate(s => s.HouseImportResultBlockErrors, ss => ss.HouseImportBlockTransportGUID, ss => ss.HouseImportBlockTransportGU);
                    e.Navigate(c => c.HouseImportResultEntrances, ss => ss.HouseImportTransportGUID, ss => ss.HouseImportTransportGU)
                        .Navigate(o => o.HouseImportResultEntranceErrors, ss => ss.HouseImportEntranceTransportGUID, ss => ss.HouseImportEntranceTransportGU);
                    e.Navigate(c => c.HouseImportResultPremises, ss => ss.HouseImportTransportGUID, ss => ss.HouseImportTransportGU)
                        .Navigate(s => s.HouseImportResultPremiseErrors, ss => ss.HouseImportPremiseTransportGUID, ss => ss.HouseImportPremiseTransportGU);
                    e.Navigate(c => c.HouseImportResultLivingRooms, ss => ss.HouseImportTransportGUID, ss => ss.HouseImportTransportGU)
                        .Navigate(o => o.HouseImportResultLivingRoomErrors, ss => ss.HouseImportLivingRoomTransportGUID, ss => ss.HouseImportLivingRoomTransportGU);
                })
                .Entity<HouseExportRequest>()
                .Entity<HouseExportResult>(e =>
                {
                    e.Navigate(c => c.HouseExportResultBlocks, ss => ss.HouseExportTransportGUID, ss => ss.HouseExportTransportGU)
                        .Navigate(ss => ss.HouseExportResultLivingRooms, ss => ss.HouseExportBlockTransportGUID, ss => ss.HouseExportBlockTransportGU);
                    e.Navigate(c => c.HouseExportResultEntrances, ss => ss.HouseExportTransportGUID, ss => ss.HouseExportTransportGU)
                        .Navigate(ss => ss.HouseExportResultPremises, ss => ss.HouseExportEntranceTransportGUID, ss => ss.HouseExportEntranceTransportGU);
                    e.Navigate(c => c.HouseExportResultPremises, ss => ss.HouseExportTransportGUID, ss => ss.HouseExportTransportGU)
                        .Navigate(ss => ss.HouseExportResultLivingRooms, ss => ss.HouseExportPremiseTransportGUID, ss => ss.HouseExportPremiseTransportGU);
                    e.Navigate(c => c.HouseExportResultLivingRooms, ss => ss.HouseExportTransportGUID, ss => ss.HouseExportTransportGU);
                });
            #endregion

            #region Notification
            entityRelationBuilder
                .Entity<NotificationImportRequest>(e =>
                {
                    e.Navigate(s => s.NotificationImportRequestAccountDebts, ss => ss.NotificationImportTransportGUID, ss => ss.NotificationImportTransportGU);
                })
                .Entity<NotificationImportDeleteRequest>()
                .Entity<NotificationImportResult>(e =>
                {
                    e.Navigate(c => c.NotificationImportResultErrors, ss => ss.NotificationImportTransportGUID, ss => ss.NotificationImportTransportGU);
                });
            #endregion

            #region Order
            entityRelationBuilder
                .Entity<OrderImportRequest>()
                .Entity<OrderImportCancellationRequest>()
                .Entity<OrderImportResult>(e =>
                {
                    e.Navigate(a => a.OrderImportResultErrors, ss => ss.OrderImportTransportGUID, ss => ss.OrderImportTransportGU);
                });
            #endregion

            #region Payment
            entityRelationBuilder
                .Entity<PaymentImportRequest>(e =>
                {
                    e.Navigate(a => a.PaymentImportRequestChargesMunicipalServices, ss => ss.PaymentImportTransportGUID, ss => ss.PaymentImportTransportGU)
                        .Navigate(s => s.PaymentImportRequestChargesMunicipalServiceNorm, ss => ss.TransportGUID, ss => ss.TransportGU);
                    e.Navigate(a => a.PaymentImportRequestPenaltyAndCourtCosts, ss => ss.PaymentImportTransportGUID, ss => ss.PaymentImportTransportGU);
                    e.Navigate(a => a.PaymentImportRequestDebtMunicipalServices, ss => ss.PaymentImportTransportGUID, ss => ss.PaymentImportTransportGU);
                })
                .Entity<PaymentImportWithdrawRequest>()
                .Entity<PaymentImportResult>(e =>
                {
                    e.Navigate(a => a.PaymentImportResultErrors, ss => ss.PaymentImportTransportGUID, ss => ss.PaymentImportTransportGU);
                })
                .Entity<PaymentExportRequest>(e =>
                {
                    e.Navigate(a => a.PaymentExportRequestDocuments, ss => ss.PaymentExportTransportGUID, ss => ss.PaymentExportTransportGU);
                    e.Navigate(a => a.PaymentExportRequestAccounts, ss => ss.PaymentExportTransportGUID, ss => ss.PaymentExportTransportGU);
                })
                .Entity<PaymentExportResult>();
            #endregion
        }

        public EntityRelationBuilder Entity<TEntity>()
        {
            EntityRelation<TEntity> entityRelation = GetOrAddEntityRelation<TEntity>();
            return this;
        }
        public EntityRelationBuilder Entity<TEntity>(Action<EntityRelation<TEntity>> buildAction)
        {
            if (buildAction == null)
            {
                throw new ArgumentNullException(nameof(buildAction));
            }

            EntityRelation<TEntity> entityRelation = GetOrAddEntityRelation<TEntity>();
            buildAction.Invoke(entityRelation);

            return this;
        }
        public EntityRelation<TEntity> GetEntityRelation<TEntity>()
        {
            IEntityRelation entityRelation;
            if (!this.entities.TryGetValue(typeof(TEntity), out entityRelation))
            {
                throw new Exception(String.Format("Не заданы зависимости для типа {0}.", typeof(TEntity)));
            }
            return (EntityRelation<TEntity>)entityRelation;
        }
        internal EntityRelation<TEntity> GetOrAddEntityRelation<TEntity>()
        {
            IEntityRelation entityRelation;
            if (!entities.TryGetValue(typeof(TEntity), out entityRelation))
            {
                entityRelation = new EntityRelation<TEntity>(this);
                entities[typeof(TEntity)] = entityRelation;
            }
            return (EntityRelation<TEntity>)entityRelation;
        }
    }

    public class EntityRelation : IEntityRelation
    {
        protected readonly IDictionary<string, Relation> NavigationStore = new Dictionary<string, Relation>();

        public Type Type { get; private set; }
        public IEnumerable<KeyValuePair<string, Relation>> Navigation
        {
            get
            {
                return this.NavigationStore;
            }
        }
        
        public EntityRelation(Type type)
        {
            this.Type = type;
        }

        private void GetTypes(IEntityRelation entityRelation, ICollection<IEntityRelation> typeList)
        {
            foreach (var child in entityRelation.Navigation)
            {
                this.GetTypes(child.Value.Reference, typeList);
            }
            typeList.Add(entityRelation);
        }
        public virtual IEnumerable<IEntityRelation> GetRelations()
        {
            ICollection<IEntityRelation> typeList = new HashSet<IEntityRelation>();
            this.GetTypes(this, typeList);
            return typeList;
        }

        // copyright
        protected static Expression RemoveConvert(Expression expression)
        {
            while (expression != null && (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked))
            {
                expression = RemoveConvert(((UnaryExpression)expression).Operand);
            }
            return expression;
        }
        // copyright
        protected static bool TryParsePath(Expression expression, out string path)
        {
            string str;
            string str1;
            string str2;
            path = null;
            Expression expression1 = RemoveConvert(expression);
            MemberExpression memberExpression = expression1 as MemberExpression;
            MethodCallExpression methodCallExpression = expression1 as MethodCallExpression;
            if (memberExpression != null)
            {
                string name = memberExpression.Member.Name;
                if (!EntityRelation.TryParsePath(memberExpression.Expression, out str))
                {
                    return false;
                }
                path = (str == null ? name : string.Concat(str, ".", name));
            }
            else if (methodCallExpression != null)
            {
                if (methodCallExpression.Method.Name == "Select" && methodCallExpression.Arguments.Count == 2)
                {
                    if (!EntityRelation.TryParsePath(methodCallExpression.Arguments[0], out str1))
                    {
                        return false;
                    }
                    if (str1 != null)
                    {
                        LambdaExpression item = methodCallExpression.Arguments[1] as LambdaExpression;
                        if (item != null)
                        {
                            if (!EntityRelation.TryParsePath(item.Body, out str2))
                            {
                                return false;
                            }
                            if (str2 != null)
                            {
                                path = string.Concat(str1, ".", str2);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            return true;
        }
    }
    
    public class EntityRelation<TEntity> : EntityRelation
    {
        private readonly EntityRelationBuilder _builder;

        public EntityRelation(EntityRelationBuilder builder)
            : base(typeof(TEntity))
        {
            _builder = builder;
        }

        public EntityRelation<TElement> Navigate<TElement>(
            Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
            Expression<Func<TElement, Guid>> referenceKey,
            Expression<Func<TElement, TEntity>> referenceProperty)
        {
            string navigationPropertyStr;
            if (!TryParsePath(navigationProperty.Body, out navigationPropertyStr) || navigationPropertyStr == null)
            {
                throw new ArgumentException("navigationProperty");
            }
            string referenceKeyStr;
            if (!TryParsePath(referenceKey.Body, out referenceKeyStr) || referenceKeyStr == null)
            {
                throw new ArgumentException("referenceKeyStr");
            }
            string referencePropertyStr;
            if (!TryParsePath(referenceProperty.Body, out referencePropertyStr) || referencePropertyStr == null)
            {
                throw new ArgumentException("referenceProperty");
            }

            Relation relation;
            if (!(this.NavigationStore.TryGetValue(navigationPropertyStr, out relation)))
            {
                relation = new Relation();
                this.NavigationStore[navigationPropertyStr] = relation;
            }
            relation.ReferenceProperty = referencePropertyStr;
            relation.ReferenceKey = referenceKeyStr;
            if (!(relation.Reference is EntityRelation<TElement>))
            {
                relation.Reference = _builder.GetOrAddEntityRelation<TElement>();
            }

            return (EntityRelation<TElement>)relation.Reference;
        }
        public EntityRelation<TElement> Navigate<TElement>(
            Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
            Expression<Func<TElement, Guid?>> referenceKey,
            Expression<Func<TElement, TEntity>> referenceProperty)
        {
            string navigationPropertyStr;
            if (!TryParsePath(navigationProperty.Body, out navigationPropertyStr) || navigationPropertyStr == null)
            {
                throw new ArgumentException("navigationProperty");
            }
            string referenceKeyStr;
            if (!TryParsePath(referenceKey.Body, out referenceKeyStr) || referenceKeyStr == null)
            {
                throw new ArgumentException("referenceKeyStr");
            }
            string referencePropertyStr;
            if (!TryParsePath(referenceProperty.Body, out referencePropertyStr) || referencePropertyStr == null)
            {
                throw new ArgumentException("referenceProperty");
            }

            Relation relation;
            if (!(this.NavigationStore.TryGetValue(navigationPropertyStr, out relation)))
            {
                relation = new Relation();
                this.NavigationStore[navigationPropertyStr] = relation;
            }
            relation.ReferenceProperty = referencePropertyStr;
            relation.ReferenceKey = referenceKeyStr;
            if (!(relation.Reference is EntityRelation<TElement>))
            {
                relation.Reference = _builder.GetOrAddEntityRelation<TElement>();
            }

            return (EntityRelation<TElement>)relation.Reference;
        }
        public EntityRelation<TElement> Navigate<TElement>(
            Expression<Func<TEntity, TElement>> navigationProperty,
            Expression<Func<TElement, Guid>> referenceKey,
            Expression<Func<TElement, TEntity>> referenceProperty)
        {
            string navigationPropertyStr;
            if (!TryParsePath(navigationProperty.Body, out navigationPropertyStr) || navigationPropertyStr == null)
            {
                throw new ArgumentException("navigationProperty");
            }
            string referenceKeyStr;
            if (!TryParsePath(referenceKey.Body, out referenceKeyStr) || referenceKeyStr == null)
            {
                throw new ArgumentException("referenceKeyStr");
            }
            string referencePropertyStr;
            if (!TryParsePath(referenceProperty.Body, out referencePropertyStr) || referencePropertyStr == null)
            {
                throw new ArgumentException("referenceProperty");
            }

            Relation relation;
            if (!(this.NavigationStore.TryGetValue(navigationPropertyStr, out relation)))
            {
                relation = new Relation();
                this.NavigationStore[navigationPropertyStr] = relation;
            }
            relation.ReferenceProperty = referencePropertyStr;
            relation.ReferenceKey = referenceKeyStr;
            if (!(relation.Reference is EntityRelation<TElement>))
            {
                relation.Reference = _builder.GetOrAddEntityRelation<TElement>();
            }

            return (EntityRelation<TElement>)relation.Reference;
        }
        public EntityRelation<TElement> Navigate<TElement>(
            Expression<Func<TEntity, TElement>> navigationProperty,
            Expression<Func<TElement, Guid?>> referenceKey,
            Expression<Func<TElement, TEntity>> referenceProperty)
        {
            string navigationPropertyStr;
            if (!TryParsePath(navigationProperty.Body, out navigationPropertyStr) || navigationPropertyStr == null)
            {
                throw new ArgumentException("navigationProperty");
            }
            string referenceKeyStr;
            if (!TryParsePath(referenceKey.Body, out referenceKeyStr) || referenceKeyStr == null)
            {
                throw new ArgumentException("referenceKeyStr");
            }
            string referencePropertyStr;
            if (!TryParsePath(referenceProperty.Body, out referencePropertyStr) || referencePropertyStr == null)
            {
                throw new ArgumentException("referenceProperty");
            }

            Relation relation;
            if (!(this.NavigationStore.TryGetValue(navigationPropertyStr, out relation)))
            {
                relation = new Relation();
                this.NavigationStore[navigationPropertyStr] = relation;
            }
            relation.ReferenceProperty = referencePropertyStr;
            relation.ReferenceKey = referenceKeyStr;
            if (!(relation.Reference is EntityRelation<TElement>))
            {
                relation.Reference = _builder.GetOrAddEntityRelation<TElement>();
            }

            return (EntityRelation<TElement>)relation.Reference;
        }
    }
}