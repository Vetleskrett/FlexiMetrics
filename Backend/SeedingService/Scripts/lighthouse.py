from fleximetrics import *
import json
import subprocess

def install_lighthouse():
    command = [
        "npm",
        "install",
        "-g",
        "lighthouse"
    ]
    subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.DEVNULL)

def run_lighthouse(url: str, output_path: str):
    command = [
        "npx",
        "lighthouse",
        url,
        "--output=json,html",
        f"--output-path={output_path}",
        '--chrome-flags="--headless --no-sandbox"',
    ]
    subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.DEVNULL)


def read_lighthouse_report(file_path: str):
    with open(file_path, "r") as file:
        report = json.load(file)
    return report


def main(entry: AssignmentEntry) -> Analysis:
    analysis = Analysis()

    if not entry.delivery:
        return analysis

    home_page_url = entry.delivery.get_url("Home Page")

    output_path = f"team{entry.team.team_nr}"
    json_path = f"{output_path}.report.json"
    html_path = f"{output_path}.report.html"

    install_lighthouse()
    run_lighthouse(home_page_url, output_path)
    report = read_lighthouse_report(json_path)

    performance = 100 * report["categories"]["performance"]["score"]
    accessibility = 100 * report["categories"]["accessibility"]["score"]
    best_practices = 100 * report["categories"]["best-practices"]["score"]

    analysis.set_filename("Report", html_path)
    analysis.set_range("Performance", performance, 100)
    analysis.set_range("Accessibility", accessibility, 100)
    analysis.set_range("Best Practices", best_practices, 100)

    return analysis


if __name__ == "__main__":
    entry = AssignmentEntry.read_from_file()
    analysis = main(entry)
    analysis.write_to_file()