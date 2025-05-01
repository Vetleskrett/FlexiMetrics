import sys
import gzip
import time
import random
import struct

def decompress_file(input_path, output_path):
    with gzip.open(input_path, 'rb') as f_in:
        bloated_data = f_in.read()

    padding_length = struct.unpack('>I', bloated_data[:4])[0]
    text = bloated_data[4 + padding_length : -padding_length].decode()

    with open(output_path, 'w') as f_out:
        f_out.write(text)


def wrong_answer(output_path):
    text = "Wrong answer"
    with open(output_path, 'w') as f_out:
        f_out.write(text)


if __name__ == "__main__":
    input_path = sys.argv[1]
    output_path = sys.argv[2]

    delay = random.random()
    time.sleep(delay)

    if random.random() > 0.1:
        decompress_file(input_path, output_path)
    else:
        wrong_answer(output_path)

    print(f"File '{input_path}' decompressed to '{output_path}'.")