﻿
ALTER TABLE "gis_hcs"."AccountExportResultPercentPremise" ADD CONSTRAINT "FK_AccountExportResultPercentPremise_0" FOREIGN KEY ("AccountExportTransportGUID") REFERENCES "gis_hcs"."AccountExportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."AccountImportRequestPayer" ADD CONSTRAINT "FK_AccountImportRequestPayer_0" FOREIGN KEY ("TransportGUID") REFERENCES "gis_hcs"."AccountImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."AccountImportRequestPercentPremise" ADD CONSTRAINT "FK_AccountImportRequestPercentPremise_0" FOREIGN KEY ("AccountImportTransportGUID") REFERENCES "gis_hcs"."AccountImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."AccountImportRequestReason" ADD CONSTRAINT "FK_AccountImportRequestReason_0" FOREIGN KEY ("AccountImportTransportGUID") REFERENCES "gis_hcs"."AccountImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."AccountImportResultError" ADD CONSTRAINT "FK_AccountImportResultError_0" FOREIGN KEY ("AccountImportTransportGUID") REFERENCES "gis_hcs"."AccountImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."AckImportResultError" ADD CONSTRAINT "FK_AckImportResultError_0" FOREIGN KEY ("AckImportTransportGUID") REFERENCES "gis_hcs"."AckImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."AttachmentPostResultCopy" ADD CONSTRAINT "FK_AttachmentPostResultCopy_0" FOREIGN KEY ("AttachmentPostTransportGUID") REFERENCES "gis_hcs"."AttachmentPostResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."ContractImportRequestAttachment" ADD CONSTRAINT "FK_ContractImportRequestAttachment_0" FOREIGN KEY ("ContractImportTransportGUID") REFERENCES "gis_hcs"."ContractImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."ContractImportRequestObjectAddress" ADD CONSTRAINT "FK_ContractImportRequestObjectAddress_0" FOREIGN KEY ("ContractImportTransportGUID") REFERENCES "gis_hcs"."ContractImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."ContractImportRequestObjectServiceResource" ADD CONSTRAINT "FK_ContractImportRequestObjectServiceResource_0" FOREIGN KEY ("ContractImportObjectAddressTransportGUID") REFERENCES "gis_hcs"."ContractImportRequestObjectAddress" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."ContractImportRequestObjectServiceResource" ADD CONSTRAINT "FK_ContractImportRequestObjectServiceResource_1" FOREIGN KEY ("ContractImportSubjectTransportGUID") REFERENCES "gis_hcs"."ContractImportRequestSubject" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."ContractImportRequestParty" ADD CONSTRAINT "FK_ContractImportRequestParty_0" FOREIGN KEY ("TransportGUID") REFERENCES "gis_hcs"."ContractImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."ContractImportRequestSubject" ADD CONSTRAINT "FK_ContractImportRequestSubject_0" FOREIGN KEY ("ContractImportTransportGUID") REFERENCES "gis_hcs"."ContractImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."ContractImportRequestSubjectQualityIndicator" ADD CONSTRAINT "FK_ContractImportRequestSubjectQualityIndicator_0" FOREIGN KEY ("ContractImportObjectAddressTransportGUID") REFERENCES "gis_hcs"."ContractImportRequestObjectAddress" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."ContractImportRequestSubjectQualityIndicator" ADD CONSTRAINT "FK_ContractImportRequestSubjectQualityIndicator_1" FOREIGN KEY ("ContractImportSubjectTransportGUID") REFERENCES "gis_hcs"."ContractImportRequestSubject" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."ContractImportResultError" ADD CONSTRAINT "FK_ContractImportResultError_0" FOREIGN KEY ("ContractImportTransportGUID") REFERENCES "gis_hcs"."ContractImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceExportResultAccount" ADD CONSTRAINT "FK_DeviceExportResultAccount_0" FOREIGN KEY ("DeviceExportTransportGUID") REFERENCES "gis_hcs"."DeviceExportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceExportResultAddress" ADD CONSTRAINT "FK_DeviceExportResultAddress_0" FOREIGN KEY ("DeviceExportTransportGUID") REFERENCES "gis_hcs"."DeviceExportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceImportReplaceRequestValue" ADD CONSTRAINT "FK_DeviceImportReplaceRequestValue_0" FOREIGN KEY ("DeviceImportReplaceTransportGUID") REFERENCES "gis_hcs"."DeviceImportReplaceRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceImportRequestAccount" ADD CONSTRAINT "FK_DeviceImportRequestAccount_0" FOREIGN KEY ("DeviceImportTransportGUID") REFERENCES "gis_hcs"."DeviceImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceImportRequestAddress" ADD CONSTRAINT "FK_DeviceImportRequestAddress_0" FOREIGN KEY ("DeviceImportTransportGUID") REFERENCES "gis_hcs"."DeviceImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceImportRequestLinkedDevice" ADD CONSTRAINT "FK_DeviceImportRequestLinkedDevice_0" FOREIGN KEY ("DeviceImportTransportGUID") REFERENCES "gis_hcs"."DeviceImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceImportRequestValue" ADD CONSTRAINT "FK_DeviceImportRequestValue_0" FOREIGN KEY ("DeviceImportTransportGUID") REFERENCES "gis_hcs"."DeviceImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceImportResultError" ADD CONSTRAINT "FK_DeviceImportResultError_0" FOREIGN KEY ("DeviceImportTransportGUID") REFERENCES "gis_hcs"."DeviceImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceValueExportRequestDevice" ADD CONSTRAINT "FK_DeviceValueExportRequestDevice_0" FOREIGN KEY ("DeviceValueExportTransportGUID") REFERENCES "gis_hcs"."DeviceValueExportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceValueExportRequestDeviceType" ADD CONSTRAINT "FK_DeviceValueExportRequestDeviceType_0" FOREIGN KEY ("DeviceValueExportTransportGUID") REFERENCES "gis_hcs"."DeviceValueExportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceValueExportRequestMunicipalResource" ADD CONSTRAINT "FK_DeviceValueExportRequestMunicipalResource_0" FOREIGN KEY ("DeviceValueExportTransportGUID") REFERENCES "gis_hcs"."DeviceValueExportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."DeviceValueImportResultError" ADD CONSTRAINT "FK_DeviceValueImportResultError_0" FOREIGN KEY ("DeviceValueImportTransportGUID") REFERENCES "gis_hcs"."DeviceValueImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseExportResultBlock" ADD CONSTRAINT "FK_HouseExportResultBlock_0" FOREIGN KEY ("HouseExportTransportGUID") REFERENCES "gis_hcs"."HouseExportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseExportResultEntrance" ADD CONSTRAINT "FK_HouseExportResultEntrance_0" FOREIGN KEY ("HouseExportTransportGUID") REFERENCES "gis_hcs"."HouseExportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseExportResultLivingRoom" ADD CONSTRAINT "FK_HouseExportResultLivingRoom_0" FOREIGN KEY ("HouseExportBlockTransportGUID") REFERENCES "gis_hcs"."HouseExportResultBlock" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseExportResultLivingRoom" ADD CONSTRAINT "FK_HouseExportResultLivingRoom_1" FOREIGN KEY ("HouseExportPremiseTransportGUID") REFERENCES "gis_hcs"."HouseExportResultPremise" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseExportResultLivingRoom" ADD CONSTRAINT "FK_HouseExportResultLivingRoom_2" FOREIGN KEY ("HouseExportTransportGUID") REFERENCES "gis_hcs"."HouseExportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseExportResultPremise" ADD CONSTRAINT "FK_HouseExportResultPremise_0" FOREIGN KEY ("HouseExportEntranceTransportGUID") REFERENCES "gis_hcs"."HouseExportResultEntrance" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseExportResultPremise" ADD CONSTRAINT "FK_HouseExportResultPremise_1" FOREIGN KEY ("HouseExportTransportGUID") REFERENCES "gis_hcs"."HouseExportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportRequestBlock" ADD CONSTRAINT "FK_HouseImportRequestBlock_0" FOREIGN KEY ("HouseImportTransportGUID") REFERENCES "gis_hcs"."HouseImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportRequestEntrance" ADD CONSTRAINT "FK_HouseImportRequestEntrance_0" FOREIGN KEY ("HouseImportTransportGUID") REFERENCES "gis_hcs"."HouseImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportRequestLivingRoom" ADD CONSTRAINT "FK_HouseImportRequestLivingRoom_0" FOREIGN KEY ("HouseImportBlockTransportGUID") REFERENCES "gis_hcs"."HouseImportRequestBlock" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportRequestLivingRoom" ADD CONSTRAINT "FK_HouseImportRequestLivingRoom_1" FOREIGN KEY ("HouseImportPremiseTransportGUID") REFERENCES "gis_hcs"."HouseImportRequestPremise" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportRequestLivingRoom" ADD CONSTRAINT "FK_HouseImportRequestLivingRoom_2" FOREIGN KEY ("HouseImportTransportGUID") REFERENCES "gis_hcs"."HouseImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportRequestPremise" ADD CONSTRAINT "FK_HouseImportRequestPremise_0" FOREIGN KEY ("HouseImportEntranceTransportGUID") REFERENCES "gis_hcs"."HouseImportRequestEntrance" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportRequestPremise" ADD CONSTRAINT "FK_HouseImportRequestPremise_1" FOREIGN KEY ("HouseImportTransportGUID") REFERENCES "gis_hcs"."HouseImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportResultBlock" ADD CONSTRAINT "FK_HouseImportResultBlock_0" FOREIGN KEY ("HouseImportTransportGUID") REFERENCES "gis_hcs"."HouseImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportResultBlockError" ADD CONSTRAINT "FK_HouseImportResultBlockError_0" FOREIGN KEY ("HouseImportBlockTransportGUID") REFERENCES "gis_hcs"."HouseImportResultBlock" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportResultEntrance" ADD CONSTRAINT "FK_HouseImportResultEntrance_0" FOREIGN KEY ("HouseImportTransportGUID") REFERENCES "gis_hcs"."HouseImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportResultEntranceError" ADD CONSTRAINT "FK_HouseImportResultEntranceError_0" FOREIGN KEY ("HouseImportEntranceTransportGUID") REFERENCES "gis_hcs"."HouseImportResultEntrance" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportResultError" ADD CONSTRAINT "FK_HouseImportResultError_0" FOREIGN KEY ("HouseImportTransportGUID") REFERENCES "gis_hcs"."HouseImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportResultLivingRoom" ADD CONSTRAINT "FK_HouseImportResultLivingRoom_0" FOREIGN KEY ("HouseImportTransportGUID") REFERENCES "gis_hcs"."HouseImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportResultLivingRoomError" ADD CONSTRAINT "FK_HouseImportResultLivingRoomError_0" FOREIGN KEY ("HouseImportLivingRoomTransportGUID") REFERENCES "gis_hcs"."HouseImportResultLivingRoom" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportResultPremise" ADD CONSTRAINT "FK_HouseImportResultPremise_0" FOREIGN KEY ("HouseImportTransportGUID") REFERENCES "gis_hcs"."HouseImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."HouseImportResultPremiseError" ADD CONSTRAINT "FK_HouseImportResultPremiseError_0" FOREIGN KEY ("HouseImportPremiseTransportGUID") REFERENCES "gis_hcs"."HouseImportResultPremise" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."NotificationImportRequestAccountDebt" ADD CONSTRAINT "FK_NotificationImportRequestAccountDebt_0" FOREIGN KEY ("NotificationImportTransportGUID") REFERENCES "gis_hcs"."NotificationImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."NotificationImportResultError" ADD CONSTRAINT "FK_NotificationImportResultError_0" FOREIGN KEY ("NotificationImportTransportGUID") REFERENCES "gis_hcs"."NotificationImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."NsiExportResultField" ADD CONSTRAINT "FK_NsiExportResultField_0" FOREIGN KEY ("NsiExportTransportGUID") REFERENCES "gis_hcs"."NsiExportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."OrderImportResultError" ADD CONSTRAINT "FK_OrderImportResultError_0" FOREIGN KEY ("OrderImportTransportGUID") REFERENCES "gis_hcs"."OrderImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."OrganizationExportRequestData" ADD CONSTRAINT "FK_OrganizationExportRequestData_0" FOREIGN KEY ("OrganizationExportTransportGUID") REFERENCES "gis_hcs"."OrganizationExportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."OrganizationExportResultEntp" ADD CONSTRAINT "FK_OrganizationExportResultEntp_0" FOREIGN KEY ("TransportGUID") REFERENCES "gis_hcs"."OrganizationExportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."OrganizationExportResultLegal" ADD CONSTRAINT "FK_OrganizationExportResultLegal_0" FOREIGN KEY ("TransportGUID") REFERENCES "gis_hcs"."OrganizationExportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."OrganizationExportResultRole" ADD CONSTRAINT "FK_OrganizationExportResultRole_0" FOREIGN KEY ("OrganizationExportTransportGUID") REFERENCES "gis_hcs"."OrganizationExportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."PaymentExportRequestAccount" ADD CONSTRAINT "FK_PaymentExportRequestAccount_0" FOREIGN KEY ("PaymentExportTransportGUID") REFERENCES "gis_hcs"."PaymentExportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."PaymentExportRequestDocument" ADD CONSTRAINT "FK_PaymentExportRequestDocument_0" FOREIGN KEY ("PaymentExportTransportGUID") REFERENCES "gis_hcs"."PaymentExportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."PaymentImportRequestChargesMunicipalService" ADD CONSTRAINT "FK_PaymentImportRequestChargesMunicipalService_0" FOREIGN KEY ("PaymentImportTransportGUID") REFERENCES "gis_hcs"."PaymentImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."PaymentImportRequestChargesMunicipalServiceNorm" ADD CONSTRAINT "FK_PaymentImportRequestChargesMunicipalServiceNorm_0" FOREIGN KEY ("TransportGUID") REFERENCES "gis_hcs"."PaymentImportRequestChargesMunicipalService" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."PaymentImportRequestDebtMunicipalService" ADD CONSTRAINT "FK_PaymentImportRequestDebtMunicipalService_0" FOREIGN KEY ("PaymentImportTransportGUID") REFERENCES "gis_hcs"."PaymentImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."PaymentImportRequestPenaltyAndCourtCost" ADD CONSTRAINT "FK_PaymentImportRequestPenaltyAndCourtCost_0" FOREIGN KEY ("PaymentImportTransportGUID") REFERENCES "gis_hcs"."PaymentImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."PaymentImportResultError" ADD CONSTRAINT "FK_PaymentImportResultError_0" FOREIGN KEY ("PaymentImportTransportGUID") REFERENCES "gis_hcs"."PaymentImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."SettlementImportRequestPeriod" ADD CONSTRAINT "FK_SettlementImportRequestPeriod_0" FOREIGN KEY ("SettlementImportTransportGUID") REFERENCES "gis_hcs"."SettlementImportRequest" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."SettlementImportRequestPeriodAnnulment" ADD CONSTRAINT "FK_SettlementImportRequestPeriodAnnulment_0" FOREIGN KEY ("TransportGUID") REFERENCES "gis_hcs"."SettlementImportRequestPeriod" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."SettlementImportRequestPeriodInfo" ADD CONSTRAINT "FK_SettlementImportRequestPeriodInfo_0" FOREIGN KEY ("TransportGUID") REFERENCES "gis_hcs"."SettlementImportRequestPeriod" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."SettlementImportResultError" ADD CONSTRAINT "FK_SettlementImportResultError_0" FOREIGN KEY ("SettlementImportTransportGUID") REFERENCES "gis_hcs"."SettlementImportResult" ("TransportGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."SysTransaction" ADD CONSTRAINT "FK_SysTransaction_0" FOREIGN KEY ("OperationId") REFERENCES "gis_hcs"."SysOperation" ("OperationId") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."SysTransactionLog" ADD CONSTRAINT "FK_SysTransactionLog_0" FOREIGN KEY ("TransactionGUID") REFERENCES "gis_hcs"."SysTransaction" ("TransactionGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."SysTransactionParam" ADD CONSTRAINT "FK_SysTransactionParam_0" FOREIGN KEY ("TransactionGUID") REFERENCES "gis_hcs"."SysTransaction" ("TransactionGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."SysTransactionState2" ADD CONSTRAINT "FK_SysTransactionState2_0" FOREIGN KEY ("OperationId") REFERENCES "gis_hcs"."SysOperation" ("OperationId") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."SysTransactionState2" ADD CONSTRAINT "FK_SysTransactionState2_1" FOREIGN KEY ("StateTypeId") REFERENCES "gis_hcs"."SysTransactionStateType" ("TransactionStateTypeId") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE "gis_hcs"."SysTransactionState2" ADD CONSTRAINT "FK_SysTransactionState2_2" FOREIGN KEY ("TransactionGUID") REFERENCES "gis_hcs"."SysTransaction" ("TransactionGUID") MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION;
