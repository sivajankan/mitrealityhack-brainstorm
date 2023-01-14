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

        // Start is called before the first frame update
        void Start()
        {

            try
            {
                // BoardShim.set_log_file("brainflow_log.txt");
                BoardShim.enable_dev_board_logger();

                BrainFlowInputParams input_params = new BrainFlowInputParams();
                // int board_id = (int)BoardIds.SYNTHETIC_BOARD;

                input_params.serial_port = "COM5";
                input_params.timeout = 5;

                // input_params.serial_number = "80E2";
                int board_id = (int)BoardIds.MUSE_S_BLED_BOARD; // is actually the MUSE 2 - they  mislabelled them internally

                // input_params.serial_number = "58C1";
                // int board_id = (int)BoardIds.MUSE_2_BLED_BOARD; // is actually the MUSE S - they  mislabelled them internally

                board_shim = new BoardShim(board_id, input_params);
                board_shim.prepare_session();
                
                // board_shim.config_board("p61"); //# to enable ppg only use p61, p50 enables aux(5th eeg) channel, ppg and smth else

                board_shim.start_stream(450000, "file://brainflow_data.csv:w");
                sampling_rate = BoardShim.get_sampling_rate(board_id);
                Debug.Log("Brainflow streaming was started");

                // MINDFULNESS= 0, DEAFULT MODEL = 0
                BrainFlowModelParams concentration_params = new BrainFlowModelParams(0, 0);
                MLModel concentration = new MLModel(concentration_params);
                concentration.prepare();

                // RESTFULNESS= 1, DEAFULT MODEL = 0
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
            double[,] data = board_shim.get_current_board_data(number_of_data_points);
            // check https://brainflow.readthedocs.io/en/stable/index.html for api ref and more code samples
            Debug.Log("Num eeg elements: " + data.GetLength(1));
            Debug.Log(String.Join(",", GetRow(data,1)));

            int[] ppg_channels = BoardShim.get_ppg_channels(21);
            Debug.Log(ppg_channels);

            // double[,] unprocessed_data = BoardShim.get_current_board_data(number_of_data_points);
            // if (unprocessed_data.GetRow(0).Length < number_of_data_points)
            // {
            //     return; // wait for more data
            // }

            // for (int i = 0; i < eeg_channels.Length; i++)
            // {
            //     filtered = DataFilter.perform_wavelet_denoising (unprocessed_data.GetRow (eeg_channels[i]), "db4", 3);
            //     // Debug.Log("channel " + eeg_channels[i] + " = " + filtered[i].ToString());
            // }

            // // prepare feature vector
            // Tuple<double[], double[]> bands = DataFilter.get_avg_band_powers (unprocessed_data, eeg_channels, sampling_rate, true);
            
            // double[] feature_vector = bands.Item1.Concatenate (bands.Item2);
            // Debug.Log("Concentration: " + concentration.predict (feature_vector)); // calc and print concetration level
            
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
                        return matrix[x, rowNumber];
                    })
                    .ToArray();
        }

        
        
    }
}
