-- Load the data in comma-separated-values format with a given schema
eurofxref =
	LOAD '$INPUT1'
	USING org.apache.pig.piggybank.storage.CSVExcelStorage
		(',', 'NO_MULTILINE', 'NOCHANGE', 'SKIP_INPUT_HEADER')
	AS (
		CURRENCY : chararray,
		TIME_PERIOD : chararray, -- can be datetime, but string works as well and it prints out better
		OBS_VALUE : double
	);

-- task 1 - max, min, avg for each currency ... (max, min with dates)
currency_group = GROUP eurofxref BY CURRENCY;
task_1 = FOREACH currency_group {
	order_desc = ORDER eurofxref BY OBS_VALUE DESC;
	max = LIMIT order_desc 1;
	order_asc = ORDER eurofxref BY OBS_VALUE ASC;
	min = LIMIT order_asc 1;
	avg = AVG(eurofxref.OBS_VALUE);
	GENERATE group as currency, FLATTEN(max.OBS_VALUE) as max_value, FLATTEN(max.TIME_PERIOD) as max_value_date,
			 FLATTEN(min.OBS_VALUE) as min_value, FLATTEN(min.TIME_PERIOD) as min_value_date, avg as avg_value;
};

-- task 2 - max, min, avg (without dates) in last year (since 2021-12-02) for currencies CHF, GBP, USD
filtered_currencies = FILTER eurofxref BY CURRENCY == 'CHF' OR CURRENCY == 'GBP' OR CURRENCY == 'USD';
filtered_year = FILTER filtered_currencies BY TIME_PERIOD >= '2021-12-02';
filtered_month = FILTER filtered_currencies BY TIME_PERIOD >= '2022-11-02';
filtered_year_group = GROUP filtered_year BY CURRENCY;
filtered_month_group = GROUP filtered_month BY CURRENCY;
agg_year = FOREACH filtered_year_group
		   GENERATE group as currency, MAX(filtered_year.OBS_VALUE) as max, AVG(filtered_year.OBS_VALUE) as avg, MIN(filtered_year.OBS_VALUE) as min;
agg_month = FOREACH filtered_month_group
		    GENERATE group as currency, MAX(filtered_month.OBS_VALUE) as max, AVG(filtered_month.OBS_VALUE) as avg, MIN(filtered_month.OBS_VALUE) as min;
joined = JOIN agg_year BY currency, agg_month BY currency;
task_2 = FOREACH joined GENERATE agg_year::currency as currency, agg_year::max as max_year, agg_year::avg as avg_year, agg_year::min as min_year,
								 agg_month::max as max_month, agg_month::avg as avg_month, agg_month::min as min_month;

-- task 3 - avg for each month for each currency of all time
cut_days = FOREACH eurofxref GENERATE CURRENCY, SUBSTRING(TIME_PERIOD, 0, 7) as TIME_PERIOD, OBS_VALUE;
cut_days_group = GROUP cut_days BY (CURRENCY, TIME_PERIOD);
task_3 = FOREACH cut_days_group GENERATE FLATTEN(group) AS (currency, date), AVG(cut_days.OBS_VALUE) as avg_value;

-- task 4 - max and min for each currecy of all time
group_all = GROUP eurofxref ALL;
max_currency = FOREACH group_all { 			  -- alternatively ORDER eurofxref BY OBS_VALUE DESC;
	ord = ORDER eurofxref BY OBS_VALUE DESC;  -- this is faster (less scalable though) 
	top = LIMIT ord 1;						  -- wanted to try an alternative approach
	GENERATE FLATTEN(top.CURRENCY) as max;  
};
min_currency = FOREACH group_all {
	ord = ORDER eurofxref BY OBS_VALUE ASC;
	top = LIMIT ord 1;
	GENERATE FLATTEN(top.CURRENCY) as min;  
};
task_4 = FOREACH max_currency GENERATE max_currency.max, min_currency.min;

-- store the output into the output directory
STORE task_1 INTO '$OUTPUT/task_1'
	USING org.apache.pig.piggybank.storage.CSVExcelStorage
		(',', 'NO_MULTILINE', 'NOCHANGE', 'WRITE_OUTPUT_HEADER');

STORE task_2 INTO '$OUTPUT/task_2'
	USING org.apache.pig.piggybank.storage.CSVExcelStorage
		(',', 'NO_MULTILINE', 'NOCHANGE', 'WRITE_OUTPUT_HEADER');

STORE task_3 INTO '$OUTPUT/task_3'
	USING org.apache.pig.piggybank.storage.CSVExcelStorage
		(',', 'NO_MULTILINE', 'NOCHANGE', 'WRITE_OUTPUT_HEADER');

STORE task_4 INTO '$OUTPUT/task_4'
	USING org.apache.pig.piggybank.storage.CSVExcelStorage
		(',', 'NO_MULTILINE', 'NOCHANGE', 'WRITE_OUTPUT_HEADER');
