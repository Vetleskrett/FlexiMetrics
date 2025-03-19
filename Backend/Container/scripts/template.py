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

    analysis.set_str("Title", "Lorem ipsum app")
    analysis.set_float("Avg file length", 134.22)
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
    analysis.set_str_list("Files", ["Lorem", "Ipsum", "Dolor", "Sit", "Amet"])
    analysis.set_int_list("Ages", [19, 21, 25])
    analysis.set_bool_list("Tests passed", [True, True, False])
    analysis.set_range_list("Scores", [95, 34, 72], 100)
    analysis.set_datetime_list(
        "Commit times", [datetime.now(), datetime.now(), datetime.now()]
    )
    analysis.set_url_list(
        "Pages",
        [
            "https://www.ntnu.no",
            "https://www.ntnu.no/studier/",
            "https://www.ntnu.no/student/trondheim",
        ],
    )
    analysis.set_json_list(
        "Commits per user",
        [
            {"Name": "Per", "Commits": 16, "Lines": 1023},
            {"Name": "Ola", "Commits": 12, "Lines": 612},
            {"Name": "Kari", "Commits": 21, "Lines": 2663},
        ],
    )

    with open("file.txt", "w") as f:
        f.write("Tempora quaerat et est ad assumenda ullam quod laudantium.")
    analysis.set_filename("My file", "file.txt")

    return analysis


if __name__ == "__main__":
    entry = AssignmentEntry.read_from_file()
    analysis = main(entry)
    analysis.write_to_file()
