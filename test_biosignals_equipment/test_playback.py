import time

import numpy as np
import pandas as pd
from brainflow.board_shim import BoardShim, BrainFlowInputParams, LogLevels, BoardIds
from brainflow.data_filter import DataFilter


BoardShim.enable_dev_board_logger()

### use synthetic board for collecting data
# params = BrainFlowInputParams()
# board = BoardShim(BoardIds.SYNTHETIC_BOARD.value, params)
# board.prepare_session()
# board.start_stream()
# BoardShim.log_message(LogLevels.LEVEL_INFO.value, 'start sleeping in the main thread')
# time.sleep(5)
# data = board.get_board_data()
# board.stop_stream()
# board.release_session()

# DataFilter.write_file(data, 'test.csv', 'w')  # use 'a' for append mode

params = BrainFlowInputParams()
params.file = "test.csv"
params.master_board = BoardIds.SYNTHETIC_BOARD
board = BoardShim(BoardIds.PLAYBACK_FILE_BOARD, params)
board.prepare_session()
board.config_board("loopback_true")
board.start_stream()
time.sleep(5)
data = board.get_board_data()
board.stop_stream()
board.release_session()

print(data)

