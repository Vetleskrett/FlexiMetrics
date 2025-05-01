from fleximetrics import *
import subprocess
import time
from faker import Faker
import os


def create_input():
    fake = Faker()
    Faker.seed(0)
    text = fake.text(max_nb_chars=100_000)
    with open("input.txt", "w") as f:
        f.write(text)


def run_command(command):
    start = time.perf_counter_ns()
    result = subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    end = time.perf_counter_ns()

    print("STDOUT:")
    print(result.stdout)
    print("STDERR:")
    print(result.stderr)

    return (end - start) // 1_000_000


def run_compression(compression_path):
    command = [
        "python",
        compression_path,
        "input.txt",
        "compressed"
    ]
    return run_command(command)


def run_decompression(decompression_path):
    command = [
        "python",
        decompression_path,
        "compressed",
        "output.txt"
    ]
    return run_command(command)


def get_compression_ratio():
    input_size = os.path.getsize("./input.txt")
    compressed_size = os.path.getsize("./compressed")
    return round(input_size / compressed_size, 2)


def get_correctnes():
    with open("input.txt", "r") as input_file:
        input_text = input_file.read()

    with open("output.txt", "r") as ouput_file:
        output_text = ouput_file.read()

    return input_text == output_text


def main(entry: AssignmentEntry) -> Analysis:
    analysis = Analysis()

    if not entry.delivery:
        return analysis

    compression_path = entry.delivery.get_filename("Compression")
    decompression_path = entry.delivery.get_filename("Decompression")

    create_input()

    compression_speed = run_compression(compression_path)
    decompression_speed = run_decompression(decompression_path)

    compression_ratio = get_compression_ratio()
    correctnes = get_correctnes()

    analysis.set_int("Compression speed", compression_speed)
    analysis.set_int("Decompression speed", decompression_speed)
    analysis.set_float("Compression Ratio", compression_ratio)
    analysis.set_bool("Correctnes", correctnes)

    return analysis


if __name__ == "__main__":
    entry = AssignmentEntry.read_from_file()
    analysis = main(entry)
    analysis.write_to_file()