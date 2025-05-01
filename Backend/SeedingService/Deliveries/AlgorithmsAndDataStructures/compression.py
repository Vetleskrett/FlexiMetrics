import sys
import gzip
import random
import time
import struct
import os

def compress_file(input_path, output_path):
    with open(input_path, 'r') as f_in:
        text = f_in.read().encode()

    padding_length = random.randint(10, 10_000)
    padding = os.urandom(padding_length)
    header = struct.pack('>I', padding_length)
    bloated_data = header + padding + text + padding

    with gzip.open(output_path, 'wb') as f_out:
        f_out.write(bloated_data)


def wrong_answer(output_path):
    text = "Wrong answer".encode()
    with gzip.open(output_path, 'wb') as f_out:
        f_out.write(text)


if __name__ == "__main__":
    input_path = sys.argv[1]
    output_path = sys.argv[2]

    delay = random.random()
    time.sleep(delay)

    if random.random() > 0.1:
        compress_file(input_path, output_path)
    else:
        wrong_answer(output_path)
    
    print(f"File '{input_path}' compressed to '{output_path}'.")
