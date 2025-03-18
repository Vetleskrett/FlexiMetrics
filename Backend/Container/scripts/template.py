from fleximetrics import *


def main(entry: AssignmentEntry) -> Analysis:
    analysis = Analysis()

    if entry.student:
        analysis.set_str("Email", entry.student.email)

    if entry.team:
        analysis.set_int("Num students", len(entry.team.students))

    if entry.delivery:
        analysis.set_bool("Has delivered", True)
        analysis.set_int("Num fields", len(entry.delivery.fields))
    else:
        analysis.set_bool("Has delivered", False)

    analysis.set_str("Title", "Weather app")
    analysis.set_int("Num files", 53)
    analysis.set_float("Avg file length", 134.22)
    analysis.set_bool("Public", True)
    analysis.set_range("Issues closed", 17, 25)
    analysis.set_datetime("Last commit", datetime.now())
    analysis.set_url("Home page", "https://www.ntnu.no")
    analysis.set_json(
        "Data",
        {
            "Value": 16,
            "Collection": ["Lorem", "Ipsum", "Dolor", "Sit", "Amet"],
            "Object": {"Value": True},
        },
    )

    return analysis


if __name__ == "__main__":
    entry = AssignmentEntry.read_from_file()
    analysis = main(entry)
    analysis.write_to_file()