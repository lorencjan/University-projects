package dist_app_environment.fielding_mapreduce;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.Mapper;
import org.apache.hadoop.mapreduce.Reducer;

import java.io.IOException;
import java.util.StringTokenizer;

public class Fielding {

    public static Job createJob(Configuration configuration) throws IOException {
        final Job job = Job.getInstance(configuration, Fielding.class.getSimpleName());
        job.setJarByClass(Fielding.class);
        job.setMapperClass(FieldingMapper.class);
        job.setCombinerClass(FieldingCombiner.class);
        job.setReducerClass(FieldingReducer.class);
        job.setOutputKeyClass(Text.class);
        job.setOutputValueClass(Text.class);
        return job;
    }

    public static class FieldingMapper extends Mapper<LongWritable, Text, Text, Text> {

        private Text newKey = new Text();
        private Text value = new Text();

        public void map(LongWritable key, Text value, Context context) throws IOException, InterruptedException {
            String[] cols = value.toString().split(",");
            String year = cols[1];
            if (year.equals(new String("yearID")) || Integer.parseInt(year) < 1995)  // skip if header or under 1995
                return;

            String playerID = cols[0];
            String teamID = cols[3];
            String games = cols[6];
    
            newKey.set(playerID + "," + teamID);
            value.set(games + ",1");

            context.write(newKey, value);            
        }
    }

    public static class FieldingCombiner extends Reducer<Text, Text, Text, Text> {

        private Text result = new Text();

        public void reduce(Text key, Iterable<Text> values, Context context) throws IOException, InterruptedException {
            int sum = 0;
            int count = 0;
            for(Text val : values) {
                String[] parts = val.toString().split(",");    // [games, count]
                sum += Integer.parseInt(parts[0]);
                count += Integer.parseInt(parts[1]);
            }
            
            result.set(String.valueOf(sum) + "," + String.valueOf(count));
            context.write(key, result);
        }
    }

    public static class FieldingReducer extends Reducer<Text, Text, Text, Text> {

        private Text result = new Text();

        public void reduce(Text key, Iterable<Text> values, Context context) throws IOException, InterruptedException {
            int sum = 0;
            double count = 0;
            for(Text val : values) {
                String[] parts = val.toString().split(",");    // [games, count]
                sum += Integer.parseInt(parts[0]);
                count += Integer.parseInt(parts[1]);
            }
            
            double avg = sum / count;
            if (avg < 10)
                return;

            result.set(String.valueOf(avg) + "," + String.valueOf(sum));
            context.write(key, result);
        }
    }

}
