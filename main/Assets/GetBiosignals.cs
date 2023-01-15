using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using brainflow;
using brainflow.math;
using UnityEngine.Events;
using System.Text;

namespace Foundry
{
    public class GetBiosignals : MonoBehaviour
    {
        private BoardShim board_shim = null;
        private int sampling_rate = 0;
        private int sampling_rate_ppg = 0;
        private double[] filtered;
        private int[] eeg_channels = null;
        private int[] ppg_channels = null;
        // private MLModel mindfulness = null;
        // private MLModel restfulness = null;

        public bool live = false;
        private bool PPG = false;
        // private bool PPG = true;
        private double[] mf_coef = {2.6338144674136394,4.006742906593334,-34.51389221061297,1.1950604401540308,35.78022137767881};
        private double mindfulness_intercept = 0.364078;
        
        public UnityEvent <double> onMindfulnessChange;
        public double mindfulness;

        // private int board_id = (int)BoardIds.SYNTHETIC_BOARD;
        // private int board_id = (int)BoardIds.MUSE_S_BLED_BOARD; // is actually the MUSE 2 - they  mislabelled them internally
        // private int board_id = (int)BoardIds.MUSE_2_BLED_BOARD; // is actually the MUSE S - they  mislabelled them internally

        // Start is called before the first frame update
        void Start()
        {
            if (live){
                try
                {                
                    // BoardShim.set_log_file("brainflow_log.txt");
                    BoardShim.enable_dev_board_logger();

                    BrainFlowInputParams input_params = new BrainFlowInputParams();

                    int board_id = (int)BoardIds.MUSE_2_BLED_BOARD; // is actually the MUSE S - they  mislabelled them internally
                    input_params.serial_port = "COM5";
                    input_params.timeout = 5;

                    board_shim = new BoardShim(board_id, input_params);
                    board_shim.prepare_session();

                    board_shim.config_board("p50"); //# to enable ppg only use p61, p50 enables aux(5th eeg) channel, ppg and smth else
                    board_shim.start_stream(450000, "file://brainflow_data_playback.csv:w");

                    sampling_rate = BoardShim.get_sampling_rate(board_id);
                    sampling_rate_ppg = BoardShim.get_sampling_rate(board_id, (int)BrainFlowPresets.ANCILLARY_PRESET);
                    eeg_channels = BoardShim.get_eeg_channels(board_id);
                    ppg_channels = BoardShim.get_ppg_channels(board_id, (int)BrainFlowPresets.ANCILLARY_PRESET);

                    Debug.Log("Brainflow streaming was started");

                    // // Brainflow Model was not cooperating, used linear classifier indirectly
                    // // MINDFULNESS= 0, DEAFULT MODEL = 0
                    // BrainFlowModelParams mindfulness_params = new BrainFlowModelParams((int)BrainFlowMetrics.MINDFULNESS, (int)BrainFlowClassifiers.DEFAULT_CLASSIFIER);
                    // MLModel mindfulness = new MLModel(mindfulness_params);
                    // mindfulness.prepare();

                    // // // RESTFULNESS= 1, DEAFULT MODEL = 0
                    // BrainFlowModelParams restfulness_params = new BrainFlowModelParams((int)BrainFlowMetrics.RESTFULNESS, (int)BrainFlowClassifiers.DEFAULT_CLASSIFIER);
                    // MLModel restfulness = new MLModel(restfulness_params);
                    // restfulness.prepare();
                }
                catch (BrainFlowError e)
                {
                    Debug.Log(e);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (live) {
                if (board_shim == null)
                {
                    return;
                }

                int number_of_data_points = sampling_rate * 6;
                double[,] data = board_shim.get_current_board_data(number_of_data_points);
                
                // Debug.Log("<------------- EEG ---------->");

                // Debug.Log("Num eeg elements rows: " + unprocessed_data.GetLength(1));
                // Debug.Log($"rows: {String.Join(",", GetRow(unprocessed_data,1))}");

                // Debug.Log("Num eeg elements cols: " + unprocessed_data.GetLength(0));
                // Debug.Log($"cols: {String.Join(",", GetCol(unprocessed_data,1))}");

                if (data.GetRow(0).Length < number_of_data_points)
                {
                    return; // wait for more data
                }

                if (PPG) 
                {
                    double[,] data_anc = board_shim.get_current_board_data(number_of_data_points, (int)BrainFlowPresets.ANCILLARY_PRESET);

                    if (data_anc.GetRow(0).Length < number_of_data_points)
                    {
                        Debug.Log($"Length of PPG: {data_anc.GetRow(0).Length} - waiting for more data");
                        return; // wait for more data
                    }

                    // Debug.Log("<------------- PPG ---------->");
                    // Debug.Log(String.Join(",", ppg_channels));

                    // Prints cols
                    // Debug.Log("Num ppg elements in a col: " + data_anc.GetLength(1));

                    // // Prints rows
                    // Debug.Log("Num ppg elements in a row: " + data_anc.GetLength(0));
                    // Debug.Log($"row: {String.Join(",", GetCol(data_anc,1))}");

                    double heart_rate = 0f;
                    try
                    {
                        heart_rate = DataFilter.get_heart_rate (data_anc.GetRow(ppg_channels[1]), data_anc.GetRow(ppg_channels[0]), sampling_rate_ppg, (int)1024);//(int)8192);
                        Debug.Log($"Heart rate: {heart_rate}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"ERROR {String.Join(",", data_anc.GetRow(ppg_channels[1]))} {String.Join(",", data_anc.GetRow(ppg_channels[0]))}");
                        // Debug.Log($"Length of ppg_ir: {data_anc.GetRow(ppg_channels[1]).GetLength(0)}");
                        // Debug.Log($"Length of ppg_red: {data_anc.GetRow(ppg_channels[2]).GetLength(0)}");
                        throw e;
                    }                
                    Debug.Log("Heart Rate: " + heart_rate);

                    // double oxygen_level = DataFilter.get_oxygen_level (data_anc.GetRow(ppg_channels[1]), data_anc.GetRow(ppg_channels[2]), sampling_rate);
                    // Debug.Log("Oxygen Level: ");
                    // Debug.Log(heart_rate);
                }

                // double[,] filtered_data = data;

                // for (int i = 0; i < eeg_channels.Length; i++)
                // {
                //     filtered_chan = DataFilter.perform_wavelet_denoising (data.GetRow(eeg_channels[i]), (int)WaveletTypes.BIOR3_9, 3);
                //     // Debug.Log("channel " + eeg_channels[i] + " = " + filtered[i].ToString());
                // }

                // prepare feature vector
                Tuple<double[], double[]> bands = DataFilter.get_avg_band_powers(data, eeg_channels, sampling_rate, true);
                // Debug.Log($"bands.Item1: {String.Join(",", bands.Item1)}");
                
                double[] feature_vector = bands.Item1;

                double value = 0.0;
                
                for (int i = 0; i < 5; i++)
                {
                    value += feature_vector[i] * mf_coef[i];
                }
                mindfulness = 1.0 / (1.0 + Mathf.Exp((float)-1.0*((float)mindfulness_intercept + (float)value)));

                Debug.Log("Mindfulness: " + mindfulness); // print mindfulness level

                onMindfulnessChange.Invoke(mindfulness);
                // SB.Append($"{mindfulness},");
            } else {
                if (mindfulness > 1) {
                    mindfulness = 0.0;
                }
                mindfulness += 0.0005;
                //mindfulness = 1;
                Debug.Log("Mindfulness: " + mindfulness); // print mindfulness level

                onMindfulnessChange.Invoke(mindfulness); 
            }
        }

        // you need to call release_session and ensure that all resources correctly released
        private void OnDestroy()
        {
            if (board_shim != null)
            {
                try
                {
                    board_shim.release_session();
                }
                catch (BrainFlowError e)
                {
                    Debug.Log(e);
                }
                Debug.Log("Brainflow streaming was stopped");
            }
        }

        private T[] GetRow<T>(T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => {
                        // Debug.Log(matrix[rowNumber, x]);
                        // return matrix[rowNumber, x];
                        return matrix[rowNumber, x];
                    })
                    .ToArray();
        }

        private T[] GetCol<T>(T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        } 
    }
}
