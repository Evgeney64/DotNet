
ALTER TABLE "public"."AccountExportResultPercentPremise"
     ADD CONSTRAINT "FK_AccountExportResultPercentPremise_AccountExportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."AccountExportResultPercentPremise" ("AccountExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."AccountImportRequestPayer"
     ADD CONSTRAINT "FK_AccountImportRequestPayer_AccountImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."AccountImportRequestPayer" ("TransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."AccountImportRequestPercentPremise"
     ADD CONSTRAINT "FK_AccountImportRequestPercentPremise_AccountImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."AccountImportRequestPercentPremise" ("AccountImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."AccountImportRequestReason"
     ADD CONSTRAINT "FK_AccountImportRequestReason_AccountImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."AccountImportRequestReason" ("AccountImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."AccountImportResultError"
     ADD CONSTRAINT "FK_AccountImportResultError_AccountImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."AccountImportResultError" ("AccountImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."AckImportResultError"
     ADD CONSTRAINT "FK_AckImportResultError_AckImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."AckImportResultError" ("AckImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."AttachmentPostResultCopy"
     ADD CONSTRAINT "FK_AttachmentPostResultCopy_AttachmentPostResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."AttachmentPostResultCopy" ("AttachmentPostTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."ContractImportRequestAttachment"
     ADD CONSTRAINT "FK_ContractImportRequestAttachment_ContractImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."ContractImportRequestAttachment" ("ContractImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."ContractImportRequestObjectAddress"
     ADD CONSTRAINT "FK_ContractImportRequestObjectAddress_ContractImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."ContractImportRequestObjectAddress" ("ContractImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."ContractImportRequestObjectServiceResource"
     ADD CONSTRAINT "FK_ContractImportRequestObjectServiceResource_ContractImportRequestObjectAddress" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."ContractImportRequestObjectServiceResource" ("ContractImportObjectAddressTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."ContractImportRequestObjectServiceResource"
     ADD CONSTRAINT "FK_ContractImportRequestObjectServiceResource_ContractImportRequestSubject" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."ContractImportRequestObjectServiceResource" ("ContractImportSubjectTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."ContractImportRequestParty"
     ADD CONSTRAINT "FK_ContractImportRequestParty_ContractImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."ContractImportRequestParty" ("TransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."ContractImportRequestSubject"
     ADD CONSTRAINT "FK_ContractImportRequestSubject_ContractImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."ContractImportRequestSubject" ("ContractImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."ContractImportRequestSubjectQualityIndicator"
     ADD CONSTRAINT "FK_ContractImportRequestSubjectQualityIndicator_ContractImportRequestObjectAddress" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."ContractImportRequestSubjectQualityIndicator" ("ContractImportObjectAddressTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."ContractImportRequestSubjectQualityIndicator"
     ADD CONSTRAINT "FK_ContractImportRequestSubjectQualityIndicator_ContractImportRequestSubject" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."ContractImportRequestSubjectQualityIndicator" ("ContractImportSubjectTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."ContractImportResultError"
     ADD CONSTRAINT "FK_ContractImportResultError_ContractImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."ContractImportResultError" ("ContractImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceExportResultAccount"
     ADD CONSTRAINT "FK_DeviceExportResultAccount_DeviceExportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceExportResultAccount" ("DeviceExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceExportResultAddress"
     ADD CONSTRAINT "FK_DeviceExportResultAddress_DeviceExportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceExportResultAddress" ("DeviceExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceImportReplaceRequestValue"
     ADD CONSTRAINT "FK_DeviceImportReplaceRequestValue_DeviceImportReplaceRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceImportReplaceRequestValue" ("DeviceImportReplaceTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceImportRequestAccount"
     ADD CONSTRAINT "FK_DeviceImportRequestAccount_DeviceImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceImportRequestAccount" ("DeviceImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceImportRequestAddress"
     ADD CONSTRAINT "FK_DeviceImportRequestAddress_DeviceImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceImportRequestAddress" ("DeviceImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceImportRequestLinkedDevice"
     ADD CONSTRAINT "FK_DeviceImportRequestLinkedDevice_DeviceImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceImportRequestLinkedDevice" ("DeviceImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceImportRequestValue"
     ADD CONSTRAINT "FK_DeviceImportRequestValue_DeviceImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceImportRequestValue" ("DeviceImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceImportResultError"
     ADD CONSTRAINT "FK_DeviceImportResultError_DeviceImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceImportResultError" ("DeviceImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceValueExportRequestDevice"
     ADD CONSTRAINT "FK_DeviceValueExportRequestDevice_DeviceValueExportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceValueExportRequestDevice" ("DeviceValueExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceValueExportRequestDeviceType"
     ADD CONSTRAINT "FK_DeviceValueExportRequestDeviceType_DeviceValueExportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceValueExportRequestDeviceType" ("DeviceValueExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceValueExportRequestMunicipalResource"
     ADD CONSTRAINT "FK_DeviceValueExportRequestMunicipalResource_DeviceValueExportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceValueExportRequestMunicipalResource" ("DeviceValueExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."DeviceValueImportResultError"
     ADD CONSTRAINT "FK_DeviceValueImportResultError_DeviceValueImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."DeviceValueImportResultError" ("DeviceValueImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseExportResultBlock"
     ADD CONSTRAINT "FK_HouseExportResultBlock_HouseExportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseExportResultBlock" ("HouseExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseExportResultEntrance"
     ADD CONSTRAINT "FK_HouseExportResultEntrance_HouseExportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseExportResultEntrance" ("HouseExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseExportResultLivingRoom"
     ADD CONSTRAINT "FK_HouseExportResultLivingRoom_HouseExportResultBlock" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseExportResultLivingRoom" ("HouseExportBlockTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseExportResultLivingRoom"
     ADD CONSTRAINT "FK_HouseExportResultLivingRoom_HouseExportResultPremise" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseExportResultLivingRoom" ("HouseExportPremiseTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseExportResultLivingRoom"
     ADD CONSTRAINT "FK_HouseExportResultLivingRoom_HouseExportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseExportResultLivingRoom" ("HouseExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseExportResultPremise"
     ADD CONSTRAINT "FK_HouseExportResultPremise_HouseExportResultEntrance" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseExportResultPremise" ("HouseExportEntranceTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseExportResultPremise"
     ADD CONSTRAINT "FK_HouseExportResultPremise_HouseExportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseExportResultPremise" ("HouseExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportRequestBlock"
     ADD CONSTRAINT "FK_HouseImportRequestBlock_HouseImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportRequestBlock" ("HouseImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportRequestEntrance"
     ADD CONSTRAINT "FK_HouseImportRequestEntrance_HouseImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportRequestEntrance" ("HouseImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportRequestLivingRoom"
     ADD CONSTRAINT "FK_HouseImportRequestLivingRoom_HouseImportRequestBlock" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportRequestLivingRoom" ("HouseImportBlockTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportRequestLivingRoom"
     ADD CONSTRAINT "FK_HouseImportRequestLivingRoom_HouseImportRequestPremise" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportRequestLivingRoom" ("HouseImportPremiseTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportRequestLivingRoom"
     ADD CONSTRAINT "FK_HouseImportRequestLivingRoom_HouseImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportRequestLivingRoom" ("HouseImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportRequestPremise"
     ADD CONSTRAINT "FK_HouseImportRequestPremise_HouseImportRequestEntrance" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportRequestPremise" ("HouseImportEntranceTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportRequestPremise"
     ADD CONSTRAINT "FK_HouseImportRequestPremise_HouseImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportRequestPremise" ("HouseImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportResultBlock"
     ADD CONSTRAINT "FK_HouseImportResultBlock_HouseImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportResultBlock" ("HouseImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportResultBlockError"
     ADD CONSTRAINT "FK_HouseImportResultBlockError_HouseImportResultBlock" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportResultBlockError" ("HouseImportBlockTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportResultEntrance"
     ADD CONSTRAINT "FK_HouseImportResultEntrance_HouseImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportResultEntrance" ("HouseImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportResultEntranceError"
     ADD CONSTRAINT "FK_HouseImportResultEntranceError_HouseImportResultEntrance" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportResultEntranceError" ("HouseImportEntranceTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportResultError"
     ADD CONSTRAINT "FK_HouseImportResultError_HouseImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportResultError" ("HouseImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportResultLivingRoom"
     ADD CONSTRAINT "FK_HouseImportResultLivingRoom_HouseImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportResultLivingRoom" ("HouseImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportResultLivingRoomError"
     ADD CONSTRAINT "FK_HouseImportResultLivingRoomError_HouseImportResultLivingRoom" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportResultLivingRoomError" ("HouseImportLivingRoomTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportResultPremise"
     ADD CONSTRAINT "FK_HouseImportResultPremise_HouseImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportResultPremise" ("HouseImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."HouseImportResultPremiseError"
     ADD CONSTRAINT "FK_HouseImportResultPremiseError_HouseImportResultPremise" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."HouseImportResultPremiseError" ("HouseImportPremiseTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."NotificationImportRequestAccountDebt"
     ADD CONSTRAINT "FK_NotificationImportRequestAccountDebt_NotificationImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."NotificationImportRequestAccountDebt" ("NotificationImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."NotificationImportResultError"
     ADD CONSTRAINT "FK_NotificationImportResultError_NotificationImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."NotificationImportResultError" ("NotificationImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."NsiExportResultField"
     ADD CONSTRAINT "FK_NsiExportResultField_NsiExportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."NsiExportResultField" ("NsiExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."OrderImportResultError"
     ADD CONSTRAINT "FK_OrderImportResultError_OrderImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."OrderImportResultError" ("OrderImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."OrganizationExportRequestData"
     ADD CONSTRAINT "FK_OrganizationExportRequestData_OrganizationExportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."OrganizationExportRequestData" ("OrganizationExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."OrganizationExportResultEntp"
     ADD CONSTRAINT "FK_OrganizationExportResultEntp_OrganizationExportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."OrganizationExportResultEntp" ("TransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."OrganizationExportResultLegal"
     ADD CONSTRAINT "FK_OrganizationExportResultLegal_OrganizationExportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."OrganizationExportResultLegal" ("TransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."OrganizationExportResultRole"
     ADD CONSTRAINT "FK_OrganizationExportResultRole_OrganizationExportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."OrganizationExportResultRole" ("OrganizationExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."PaymentExportRequestAccount"
     ADD CONSTRAINT "FK_PaymentExportRequestAccount_PaymentExportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."PaymentExportRequestAccount" ("PaymentExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."PaymentExportRequestDocument"
     ADD CONSTRAINT "FK_PaymentExportRequestDocument_PaymentExportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."PaymentExportRequestDocument" ("PaymentExportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."PaymentImportRequestChargesMunicipalService"
     ADD CONSTRAINT "FK_PaymentImportRequestChargesMunicipalService_PaymentImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."PaymentImportRequestChargesMunicipalService" ("PaymentImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."PaymentImportRequestChargesMunicipalServiceNorm"
     ADD CONSTRAINT "FK_PaymentImportRequestChargesMunicipalServiceNorm_PaymentImportRequestChargesMunicipalService" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."PaymentImportRequestChargesMunicipalServiceNorm" ("TransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."PaymentImportRequestDebtMunicipalService"
     ADD CONSTRAINT "FK_PaymentImportRequestDebtMunicipalService_PaymentImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."PaymentImportRequestDebtMunicipalService" ("PaymentImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."PaymentImportRequestPenaltyAndCourtCost"
     ADD CONSTRAINT "FK_PaymentImportRequestPenaltyAndCourtCost_PaymentImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."PaymentImportRequestPenaltyAndCourtCost" ("PaymentImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."PaymentImportResultError"
     ADD CONSTRAINT "FK_PaymentImportResultError_PaymentImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."PaymentImportResultError" ("PaymentImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."SettlementImportRequestPeriod"
     ADD CONSTRAINT "FKSettlementImportRequestPeriod_SettlementImportRequest" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."SettlementImportRequestPeriod" ("SettlementImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."SettlementImportRequestPeriodAnnulment"
     ADD CONSTRAINT "FK_SettlementImportRequestPeriodAnnulment_SettlementImportRequestPeriod" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."SettlementImportRequestPeriodAnnulment" ("TransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."SettlementImportRequestPeriodInfo"
     ADD CONSTRAINT "FK_SettlementImportRequestPeriodInfo_SettlementImportRequestPeriod" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."SettlementImportRequestPeriodInfo" ("TransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."SettlementImportResultError"
     ADD CONSTRAINT "FK_SettlementImportResultError_SettlementImportResult" FOREIGN KEY ("TransportGUID")
     REFERENCES "public"."SettlementImportResultError" ("SettlementImportTransportGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."SysTransaction"
     ADD CONSTRAINT "FK_SysTransaction_SysOperation" FOREIGN KEY ("OperationId")
     REFERENCES "public"."SysTransaction" ("OperationId") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."SysTransactionLog"
     ADD CONSTRAINT "FK_SysTransactionLog_SysTransaction" FOREIGN KEY ("TransactionGUID")
     REFERENCES "public"."SysTransactionLog" ("TransactionGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."SysTransactionParam"
     ADD CONSTRAINT "FK_SysTransactionParam_SysTransaction" FOREIGN KEY ("TransactionGUID")
     REFERENCES "public"."SysTransactionParam" ("TransactionGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."SysTransactionState2"
     ADD CONSTRAINT "FK_SysTransactionState2_SysOperation" FOREIGN KEY ("OperationId")
     REFERENCES "public"."SysTransactionState2" ("OperationId") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."SysTransactionState2"
     ADD CONSTRAINT "FK_SysTransactionState2_SysTransactionStateType" FOREIGN KEY ("TransactionStateTypeId")
     REFERENCES "public"."SysTransactionState2" ("StateTypeId") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


ALTER TABLE "public"."SysTransactionState2"
     ADD CONSTRAINT "FK_SysTransactionState2_SysTransaction" FOREIGN KEY ("TransactionGUID")
     REFERENCES "public"."SysTransactionState2" ("TransactionGUID") MATCH SIMPLE
     ON UPDATE NO ACTION
     ON DELETE NO ACTION;


