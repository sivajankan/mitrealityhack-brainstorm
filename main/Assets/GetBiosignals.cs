using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using brainflow;
using brainflow.math;

namespace Foundry
{
    public class GetBiosignals : MonoBehaviour
    {
        private BoardShim board_shim = null;
        private int sampling_rate = 0;
        private double[] filtered;
        private int[] eeg_channels = null;
        private int[] ppg_channels = null;
        private MLModel mindfulness = null;
        private MLModel restfulness = null;

        private bool PPG = true;

        // private int board_id = (int)BoardIds.SYNTHETIC_BOARD;
        private int board_id = (int)BoardIds.MUSE_S_BLED_BOARD; // is actually the MUSE 2 - they  mislabelled them internally
        // private int board_id = (int)BoardIds.MUSE_2_BLED_BOARD; // is actually the MUSE S - they  mislabelled them internally

        // Start is called before the first frame update
        void Start()
        {

            try
            {
                // BoardShim.set_log_file("brainflow_log.txt");
                BoardShim.enable_dev_board_logger();

                BrainFlowInputParams input_params = new BrainFlowInputParams();

                input_params.serial_port = "COM5";
                input_params.timeout = 5;

                // input_params.serial_number = "80E2"; // Muse 2
                // input_params.serial_number = "58C1"; // Muse S

                board_shim = new BoardShim(board_id, input_params);
                board_shim.prepare_session();
                
                board_shim.config_board("p50"); //# to enable ppg only use p61, p50 enables aux(5th eeg) channel, ppg and smth else

                board_shim.start_stream(450000, "file://brainflow_data.csv:w");
                sampling_rate = BoardShim.get_sampling_rate(board_id);
                eeg_channels = BoardShim.get_eeg_channels(board_id);
                ppg_channels = BoardShim.get_ppg_channels(board_id, (int)BrainFlowPresets.ANCILLARY_PRESET);
                
                Debug.Log("Brainflow streaming was started");

                // MINDFULNESS= 0, DEAFULT MODEL = 0
                BrainFlowModelParams mindfulness_params = new BrainFlowModelParams(0, 0);
                MLModel mindfulness = new MLModel(mindfulness_params);
                mindfulness.prepare();

                // // RESTFULNESS= 1, DEAFULT MODEL = 0
                BrainFlowModelParams restfulness_params = new BrainFlowModelParams(0, 0);
                MLModel restfulness = new MLModel(restfulness_params);
                restfulness.prepare();
            }
            catch (BrainFlowError e)
            {
                Debug.Log(e);
            }
        }

        // Update is called once per frame
        void Update()
        {
        
            if (board_shim == null)
            {
                return;
            }

            int number_of_data_points = sampling_rate * 4;
            double[,] unprocessed_data = board_shim.get_current_board_data(number_of_data_points);
            
            // Debug.Log("<------------- EEG ---------->");

            // actually prints cols
            Debug.Log("Num eeg elements rows: " + unprocessed_data.GetLength(1));
            Debug.Log("rows: ");
            Debug.Log(String.Join(",", GetRow(unprocessed_data,1)));

            // actually prints rows
            Debug.Log("Num eeg elements cols: " + unprocessed_data.GetLength(0));
            Debug.Log("cols: ");
            Debug.Log(String.Join(",", GetCol(unprocessed_data,1)));

            if (unprocessed_data.GetRow(0).Length < number_of_data_points)
            {
                return; // wait for more data
            }

            if (PPG) 
            {
                // double[,] data_anc = board_shim.get_current_board_data(number_of_data_points, (int)BrainFlowPresets.ANCILLARY_PRESET);
                double[,] data_anc = board_shim.get_current_board_data(number_of_data_points, (int)2);

                Debug.Log(String.Join(",", ppg_channels));

                Debug.Log("<------------- PPG ---------->");

                // for whatever reason - can't seem to get to print out - but I know there are channels and data!

                // actually prints cols
                Debug.Log("Num ppg elements rows: " + data_anc.GetLength(1));
                Debug.Log("rows: ");
                Debug.Log(String.Join(",", GetRow(data_anc,1)));

                // // actually prints rows
                Debug.Log("Num ppg elements cols: " + data_anc.GetLength(0));
                Debug.Log("cols: ");
                Debug.Log(String.Join(",", GetCol(data_anc,1)));
                    
                double heart_rate = DataFilter.get_heart_rate (data_anc.GetRow(ppg_channels[1]), data_anc.GetRow(ppg_channels[2]), sampling_rate, (int)8192);
                Debug.Log("Heart Rate: " + heart_rate.ToString("F4"));

                double oxygen_level = DataFilter.get_oxygen_level (data_anc.GetRow(ppg_channels[1]), data_anc.GetRow(ppg_channels[2]), sampling_rate);
                Debug.Log("Oxygen Level: ");
                Debug.Log(heart_rate);
            }

            for (int i = 0; i < eeg_channels.Length; i++)
            {
                filtered = DataFilter.perform_wavelet_denoising (unprocessed_data.GetRow(eeg_channels[i]), (int)WaveletTypes.BIOR3_9, 3);
                // Debug.Log("channel " + eeg_channels[i] + " = " + filtered[i].ToString());
            }

            // prepare feature vector
            Tuple<double[], double[]> bands = DataFilter.get_avg_band_powers (unprocessed_data, eeg_channels, sampling_rate, true);
            // Tuple<double[], double[]> bands = DataFilter.get_avg_band_powers (filtered, eeg_channels, sampling_rate, true);
            
            double[] feature_vector = bands.Item1.Concatenate (bands.Item2);
            Debug.Log("Mindfulness: " + mindfulness.predict(feature_vector)); // calc and print mindfulness level

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
