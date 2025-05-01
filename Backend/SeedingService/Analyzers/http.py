from fleximetrics import *
import tarfile
import os
import subprocess
import time
import requests

def extract_src(srcTarPath):
    os.makedirs("src", exist_ok=True)
    with tarfile.open(srcTarPath, "r:gz") as tar:
        tar.extractall(path="src", filter=None)

def build():
    command = [
        "g++",
        "src/main.cpp",
        "-o",
        "src/main.out" 
    ]

    result = subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    
    if result.stdout:
        print("STDOUT:")
        print(result.stdout)
    if result.stderr:
        print("STDERR:")
        print(result.stderr)

def run():
    command = [
        "src/main.out"
    ]
    subprocess.Popen(command)
    time.sleep(2)

def test_post():
    item = {
        "name": "Item 1"
    }
    return requests.post("http://localhost:8080/api/items", json=item)

def test_get():
    return requests.get("http://localhost:8080/api/items")


def main(entry: AssignmentEntry) -> Analysis:
    analysis = Analysis()

    if not entry.delivery:
        return analysis

    srcTarPath = entry.delivery.get_filename("Source Code (.tar.gz)")

    extract_src(srcTarPath)
    build()
    run()

    post_response = test_post()
    get_response = test_get()

    analysis.set_int("POST Status Code", post_response.status_code)
    analysis.set_int("GET Status Code", get_response.status_code)
    analysis.set_json("GET Content", get_response.json())

    return analysis


if __name__ == "__main__":
    entry = AssignmentEntry.read_from_file()
    analysis = main(entry)
    analysis.write_to_file()