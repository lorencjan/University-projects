package dist_app_environment.fielding_mapreduce;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.lib.input.FileInputFormat;
import org.apache.hadoop.mapreduce.lib.output.FileOutputFormat;
import org.apache.hadoop.util.GenericOptionsParser;

public class DriverFull {

    public static void main(String[] args) throws Exception {
        // disable IPv6: not supported by Hadoop
        System.setProperty("java.net.preferIPv4Stack", "true");
        // conf
        final Configuration conf = new Configuration();
        final String[] otherArgs = new GenericOptionsParser(conf, args).getRemainingArgs();
        if (otherArgs.length < 2) {
            System.err.println("Usage: fielding <hdfs_in> [<hdfs_in>...] <hdfs_out>");
            System.exit(2);
        }
        // job
        final Job job = Fielding.createJob(conf);
        for (int i = 0; i < otherArgs.length - 1; ++i) {
            FileInputFormat.addInputPath(job, new Path(otherArgs[i]));
        }
        FileOutputFormat.setOutputPath(job, new Path(otherArgs[otherArgs.length - 1]));
        job.waitForCompletion(true);
        // exit
        System.exit(job.isSuccessful() ? 0 : 1);
    }

}
