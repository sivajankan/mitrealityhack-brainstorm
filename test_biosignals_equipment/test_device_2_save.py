import time

import numpy as np
import pandas as pd
from brainflow.board_shim import BoardShim, BrainFlowInputParams, LogLevels, BoardIds
from brainflow.data_filter import DataFilter


def main():
    BoardShim.enable_dev_board_logger()

    # use synthetic board for demo
    params = BrainFlowInputParams()
    params.serial_port = "COM5"
    board_id = BoardIds.MUSE_2_BLED_BOARD.value

    eeg_channels = BoardShim.get_eeg_channels(board_id)
    ppg_channels = BoardShim.get_ppg_channels(board_id, 2)
    print(eeg_channels)
    print(ppg_channels)


    board = BoardShim(board_id, params)
    board.prepare_session()
    board.config_board("p50")

    board.start_stream()
    BoardShim.log_message(LogLevels.LEVEL_INFO.value, 'start sleeping in the main thread')
    print(BoardShim.get_board_descr(board_id))
    time.sleep(10)

    data_anc = board.get_current_board_data(200, 2)

    heart_rate = DataFilter.get_heart_rate(data_anc[1,:],data_anc[2,:],200, 8192)
    print(heart_rate)

    data = board.get_board_data()
    board.stop_stream()
    board.release_session()

    # demo how to convert it to pandas DF and plot data

    df = pd.DataFrame(np.transpose(data))
    print('Data From the Board')
    print(df.head(10))

    # demo for data serialization using brainflow API, we recommend to use it instead pandas.to_csv()
    DataFilter.write_file(data, 'test.csv', 'w')  # use 'a' for append mode
    restored_data = DataFilter.read_file('test.csv')
    restored_df = pd.DataFrame(np.transpose(restored_data))
    print('Data From the File')
    print(restored_df.head(10))

if __name__ == "__main__":
    main()