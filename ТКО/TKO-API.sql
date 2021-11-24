select alg.NALGORITHM_ID, alg.NALGORITHM_NAME, impl.HANDLER_LIB, impl.HANDLER_TYPE, impl.HANDLER_METHOD, impl.MFY_SUSER_ID, impl.DEVELOPER, impl.COMMENT
from NSI_ALGORITHM alg
	inner join NSI_ALGORITHM_IMPL impl on impl.NALGORITHM_ID=alg.NALGORITHM_ID
where alg.NALGORITHM_ID in (630, 647, 610, 614, 482)
order by impl.HANDLER_LIB, impl.HANDLER_TYPE, impl.HANDLER_METHOD


610	Импорт реестра оплат из 1С							Tsb.Algorithms.Calc	Tsb.Algorithms.Payments.ImportPaymentReestr		ImportPaymentReestrFrom1c_610
614	Выгрузка 1С: Счета - фактуры						Tsb.Tasks.Expend	Tsb.Tasks.Expend.CoreServices					OneC_Invoice
482	Выгрузка 1С: Контрагенты и договоры					Tsb.Tasks.Expend	Tsb.Tasks.Expend.CoreServices					OneC_Partner
630	Маджента: Загрузка контейнерных площадок			Tsb.Tasks.SamaraTKO	Tsb.Tasks.SamaraTKO.MagentaImportContainer		Run
647	Маджента: Загрузка графиков фактического вывоза		Tsb.Tasks.SamaraTKO	Tsb.Tasks.SamaraTKO.MagentaImportContainerFact	Run
