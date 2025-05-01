from fleximetrics import *
import tarfile
import os
import subprocess
import pathvalidate

def extract_src(srcTarPath):
    os.makedirs("src", exist_ok=True)
    with tarfile.open(srcTarPath, "r:gz") as tar:
        tar.extractall(path="src", filter=None)

def analyze():
    command = [
        "cppcheck",
        "--enable=all",
        "src/main.cpp"
    ]

    result = subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    
    if result.stdout:
        print("STDOUT:")
        print(result.stdout)
    if result.stderr:
        print("STDERR:")
        print(result.stderr)

    return result.stderr

def analyze_xml():
    command = [
        "cppcheck",
        "--enable=all",
        "--xml",
        "--xml-version=2",
        "src/main.cpp"
    ]

    result = subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    
    if result.stdout:
        print("STDOUT:")
        print(result.stdout)
    if result.stderr:
        print("STDERR:")
        print(result.stderr)

    return result.stderr


def main(entry: AssignmentEntry) -> Analysis:
    analysis = Analysis()

    if not entry.delivery:
        return analysis

    srcTarPath = entry.delivery.get_filename("Source Code (.tar.gz)")

    extract_src(srcTarPath)
    text_report = analyze()
    xml_report = analyze_xml()

    file_path = pathvalidate.sanitize_filename(f"team{entry.student.name}.report.xml")
    with open(file_path, "w") as f:
        f.write(xml_report)

    analysis.set_str("Output", text_report)
    analysis.set_filename("XML Report", file_path)

    return analysis


if __name__ == "__main__":
    entry = AssignmentEntry.read_from_file()
    analysis = main(entry)
    analysis.write_to_file()