select * from NSI_FACILITY where NFACILITY_ID in (1, 3, 4, 5, 6, 7, 29, 25, 27, 28, 26)
select * from NSI_FACILITY where NFACILITY_ID in (20, 21, 22, 23)
select * from NSI_CALC_ITEM where NCALC_ITEM_ID in (1, 7)


-- ОН
select cli.NCALC_ITEM_ID, fac.NFACILITY_ID, nfac.NFACILITY_NAME, count(*) 
from FACILITY fac
	inner join FACILITY_PRODUCT fpr on fpr.FACILITY_ID=fac.FACILITY_ID and fpr.NPRODUCT_ID = 7
	inner join CALC_ITEM cli on cli.FACILITY_PRODUCT_ID=fpr.FACILITY_PRODUCT_ID-- and cli.NCALC_ITEM_ID=7
	inner join NSI_FACILITY nfac on nfac.NFACILITY_ID=fac.NFACILITY_ID
group by cli.NCALC_ITEM_ID, fac.NFACILITY_ID, nfac.NFACILITY_NAME
order by cli.NCALC_ITEM_ID, fac.NFACILITY_ID


-- ОН и сделка
select fac.FACILITY_ID, fac.PARENT_ID, con.DEAL_ID, cli.CALC_ITEM_ID, *
from FACILITY fac
	inner join FACILITY_PRODUCT fpr on fpr.FACILITY_ID=fac.FACILITY_ID and fpr.NPRODUCT_ID = 7
	inner join CALC_ITEM cli on cli.FACILITY_PRODUCT_ID=fpr.FACILITY_PRODUCT_ID
	inner join NSI_FACILITY nfac on nfac.NFACILITY_ID=fac.NFACILITY_ID
	inner join CONSUM con on con.FACILITY_ID=fac.FACILITY_ID
where cli.NCALC_ITEM_ID=1 and con.DEAL_ID=2020100000000103862 
order by cli.NCALC_ITEM_ID, fac.NFACILITY_ID


-- Сделка / ОН / МН
select dl.DEAL_ID, dl.DEAL_NUM, --fac.PARENT_ID, 
	fac.FACILITY_ID, fac.FACILITY_NAME, nfac.NFACILITY_NAME, cli.CALC_ITEM_ID, cli.CALC_ITEM_NAME,
	chld.FACILITY_ID as ch_FACILITY_ID, chld.FACILITY_NAME as ch_FACILITY_NAME, chld.NFACILITY_NAME as NFACILITY_NAME_ch, chld.CALC_ITEM_ID as ch_CALC_ITEM_ID, chld.CALC_ITEM_NAME as ch_CALC_ITEM_NAME,-- chld.PARENT_ID as PARENT_ID_ch,
	fac.ADDRESS, sch.CALC_ITEM_SCHEM_ID
	--, *
from FACILITY fac
	inner join FACILITY_PRODUCT fpr on fpr.FACILITY_ID=fac.FACILITY_ID and fpr.NPRODUCT_ID = 7
	inner join CALC_ITEM cli on cli.FACILITY_PRODUCT_ID=fpr.FACILITY_PRODUCT_ID and cli.NCALC_ITEM_ID=1
	inner join NSI_FACILITY nfac on nfac.NFACILITY_ID=fac.NFACILITY_ID
	inner join CONSUM con on con.FACILITY_ID=fac.FACILITY_ID
	inner join DEAL dl on dl.DEAL_ID=con.DEAL_ID
	inner join CALC_ITEM_SCHEM sch on sch.PARENT_ID=cli.CALC_ITEM_ID
	inner join 
		(select fac.FACILITY_ID, fac.PARENT_ID, nfac.NFACILITY_NAME, cli.CALC_ITEM_ID, fac.FACILITY_NAME, cli.CALC_ITEM_NAME
		from FACILITY fac
			inner join FACILITY_PRODUCT fpr on fpr.FACILITY_ID=fac.FACILITY_ID and fpr.NPRODUCT_ID = 7
			inner join CALC_ITEM cli on cli.FACILITY_PRODUCT_ID=fpr.FACILITY_PRODUCT_ID
			inner join NSI_FACILITY nfac on nfac.NFACILITY_ID=fac.NFACILITY_ID
		where cli.NCALC_ITEM_ID=7
		) chld on chld.CALC_ITEM_ID=sch.CHILD_ID
where fpr.NPRODUCT_ID=7 and con.DEAL_ID=2020100000000103862 
order by con.DEAL_ID, cli.NCALC_ITEM_ID, fac.NFACILITY_ID


-- Алгоритмы
select alg.NALGORITHM_ID, alg.NALGORITHM_NAME, impl.HANDLER_LIB, impl.HANDLER_TYPE, impl.HANDLER_METHOD, impl.MFY_SUSER_ID, impl.DEVELOPER, impl.COMMENT
from NSI_ALGORITHM alg
	inner join NSI_ALGORITHM_IMPL impl on impl.NALGORITHM_ID=alg.NALGORITHM_ID
where alg.NALGORITHM_ID in (630, 647, 610, 614, 482)
order by impl.HANDLER_LIB, impl.HANDLER_TYPE, impl.HANDLER_METHOD


-- Изменения после заданной даты
select distinct MFY_DATE, tbl from 
	(
	select MFY_DATE, 'DEAL' as tbl from DEAL 
	union
	select MFY_DATE, 'DEAL_EXT' from DEAL_EXT 
	union
	select MFY_DATE, 'FACILITY' from FACILITY 
	union
	select MFY_DATE, 'FACILITY_EXT' from FACILITY_EXT 
	union
	select MFY_DATE, 'CALC_ITEM' from CALC_ITEM 
	union
	select MFY_DATE, 'CALC_ITEM_EXT' from CALC_ITEM_EXT 
	) un
where MFY_DATE > '20211001'
order by MFY_DATE
