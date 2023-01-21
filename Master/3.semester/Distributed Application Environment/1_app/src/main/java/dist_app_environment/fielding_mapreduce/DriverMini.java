package dist_app_environment.fielding_mapreduce;

import com.google.common.io.Files;
import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.FileStatus;
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.hdfs.HdfsConfiguration;
import org.apache.hadoop.hdfs.MiniDFSCluster;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.lib.input.FileInputFormat;
import org.apache.hadoop.mapreduce.lib.output.FileOutputFormat;
import org.apache.hadoop.util.GenericOptionsParser;

import java.io.File;
import java.util.LinkedList;
import java.util.List;

public class DriverMini {

    private final static File MINIDFS_BASEDIR = Files.createTempDir();
    private final static Path DFS_INPUT_DIR = new Path("fielding/input");
    private final static Path DFS_OUTPUT_DIR = new Path("fielding/output");

    public static void main(String[] args) throws Exception {
        // disable IPv6: not supported by Hadoop and results into error "Relative path in absolute URI" for Block Pool
        // `hostname` must have an IPv4 address, check it by: host -vt A `hostname`
        System.setProperty("java.net.preferIPv4Stack", "true");
        // conf
        final Configuration conf = new HdfsConfiguration();
        final String[] otherArgs = new GenericOptionsParser(conf, args).getRemainingArgs();
        if (otherArgs.length < 2) {
            System.err.println("Usage: fielding <local_in> [<local_in>...] <local_out>");
            System.exit(2);
        }
        // cluster start
        conf.set(MiniDFSCluster.HDFS_MINIDFS_BASEDIR, MINIDFS_BASEDIR.getAbsolutePath());
        final MiniDFSCluster miniDFSCluster = new MiniDFSCluster.Builder(conf).build();
        final FileSystem distributedFileSystem = miniDFSCluster.getFileSystem();
        final FileSystem localFileSystem = FileSystem.getLocal(conf);
        // HDFS init directories
        distributedFileSystem.delete(DFS_INPUT_DIR, true);
        distributedFileSystem.delete(DFS_OUTPUT_DIR, true);
        distributedFileSystem.mkdirs(DFS_INPUT_DIR);
        // HDFS copy input files from the local filesystem
        final Path[] srcPaths;
        // in Java 8:
        //srcPaths = IntStream.range(0, otherArgs.length - 1).mapToObj(i -> new Path(otherArgs[i])).toArray(Path[]::new);
        // in Java 7:
        final List<Path> srcPathsList = new LinkedList<>();
        for (int i = 0; i < otherArgs.length - 1; i++) {
            srcPathsList.add(new Path(otherArgs[i]));
        }
        srcPaths = new Path[srcPathsList.size()];
        srcPathsList.toArray(srcPaths);
        //
        distributedFileSystem.copyFromLocalFile(false, true, srcPaths, DFS_INPUT_DIR);
        // job
        final Job job = Fielding.createJob(conf);
        FileInputFormat.addInputPath(job, DFS_INPUT_DIR);
        FileOutputFormat.setOutputPath(job, DFS_OUTPUT_DIR);
        job.waitForCompletion(true);
        // HDFS dump resulting files to the local filesystem
        if (job.isSuccessful()) {
            final Path outputPath = new Path(otherArgs[otherArgs.length - 1]);
            localFileSystem.delete(outputPath, true);
            localFileSystem.mkdirs(outputPath);
            for (FileStatus file : distributedFileSystem.listStatus(DFS_OUTPUT_DIR)) {
                final Path filePath = file.getPath();
                distributedFileSystem.copyToLocalFile(true, filePath, new Path(outputPath, filePath.getName()));
            }
        }
        // cluster stop
        miniDFSCluster.shutdown(true);
    }

}
