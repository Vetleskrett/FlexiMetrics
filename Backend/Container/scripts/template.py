from fleximetrics import *


def main(entry: AssignmentEntry) -> Analysis:
    analysis = Analysis()

    analysis.set_int("Num files", 5)
    analysis.set_str("Title", "Weather app")

    return analysis


if __name__ == "__main__":
    entry = AssignmentEntry.read_from_file()
    analysis = main(entry)
    analysis.write_to_file()