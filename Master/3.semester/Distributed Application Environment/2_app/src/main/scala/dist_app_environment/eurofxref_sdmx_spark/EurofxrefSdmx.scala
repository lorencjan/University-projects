package dist_app_environment.eurofxref_sdmx_spark

import java.io.File

import org.apache.commons.io.FileUtils
import org.apache.spark.launcher.SparkLauncher
import org.apache.spark.sql.SparkSession
import org.apache.spark.sql.expressions.Window
import org.apache.spark.sql.functions._

object EurofxrefSdmx {
  def main(args: Array[String]) {
    if (args.length < 2) {
      System.err.println("Usage: eurofxref-sdmx <hdfs_in> [<hdfs_in>...] <hdfs_out>\n"
        + "hdfs_* args should start with hdfs:// for an HDFS filesystem or file:// for a local filesystem")
      System.exit(2)
    }

    // environment variable of Spark master node defaults to local[*] if the the master URL is absent
    if (System.getProperty(SparkLauncher.SPARK_MASTER) == null)
      System.setProperty(SparkLauncher.SPARK_MASTER, "local[*]")
    
    // create a Spark session for the job with respect to the environment variable above
    val spark = SparkSession.builder.appName(EurofxrefSdmx.getClass.getSimpleName).getOrCreate()
    import spark.implicits._

    // read files (based on "fs.defaultFS" for absent schema, "hdfs://..." if set HADOOP_CONF_DIR env-var, or "file://")
    val df = spark.read.format("csv")
      .option("header", "true") // uses the first line as names of columns
      .option("inferSchema", "true") // infers the input schema automatically from data (one extra pass over the data)
      .load(args.dropRight(1): _*)
      .withColumnRenamed("CURRENCY", "currency")
      .withColumnRenamed("TIME_PERIOD", "date")
      .withColumnRenamed("OBS_VALUE", "value")
    
    // task 1 - max, min, avg for each currency ... (max, min with dates)
    val partition = Window.partitionBy("currency");
    val partition_desc = partition.orderBy(col("value").desc)
    val partition_asc = partition.orderBy(col("value").asc)
    val max_df = df
                  .withColumn("row", row_number.over(partition_desc))
                  .where($"row" === 1)
                  .drop("row")
                  .withColumnRenamed("date", "max_value_date")
                  .withColumnRenamed("value", "max_value")

    val min_df = df
                  .withColumn("row", row_number.over(partition_asc))
                  .where($"row" === 1)
                  .drop("row")
                  .withColumnRenamed("currency", "min_currency")
                  .withColumnRenamed("date", "min_value_date")
                  .withColumnRenamed("value", "min_value")

    val avg_df = df.groupBy("currency").avg("value").withColumnRenamed("avg(value)", "avg_value").withColumnRenamed("currency", "avg_currency")
    val task_1_df = max_df.join(min_df, $"currency" === $"min_currency", "right").drop("min_currency")
                          .join(avg_df, $"currency" === $"avg_currency", "right").drop("avg_currency")
                          .sort("currency")

    // task 2 - max, min, avg (without dates) in last year (since 2021-12-02) for currencies CHF, GBP, USD
    val filtered_currencies_df = df.filter($"currency" === "CHF" || $"currency" === "GBP" || $"currency" === "USD")
    val filtered_year_df = filtered_currencies_df.filter($"date" >= "2021-12-02")
    val filtered_month_df = filtered_currencies_df.filter($"date" >= "2022-11-02")
    val agg_year = filtered_year_df
                    .groupBy("currency")
                    .agg(max("value").alias("max_year"), avg("value").alias("avg_year"), min("value").alias("min_year"))
    val agg_month = filtered_month_df
                    .groupBy("currency")
                    .agg(max("value").alias("max_month"), avg("value").alias("avg_month"), min("value").alias("min_month"))
                    .withColumnRenamed("currency", "currency_month")
    val task_2_df = agg_year.join(agg_month, $"currency" === $"currency_month", "right").drop("currency_month").sort("currency")
    
    // dump results
    val task_1_path = args.last + "/task_1"
    FileUtils.deleteDirectory(new File(task_1_path))
    task_1_df.coalesce(1).write.format("csv").option("header", "true").save(task_1_path)

    val task_2_path = args.last + "/task_2"
    FileUtils.deleteDirectory(new File(task_2_path))
    task_2_df.coalesce(1).write.format("csv").option("header", "true").save(task_2_path)
  }
}
