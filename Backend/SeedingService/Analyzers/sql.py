from fleximetrics import *
import sqlite3
from eralchemy import render_er

def generate_db(sql):
    with sqlite3.connect("my.db") as connection:
        connection.executescript(sql)

def generate_diagram(diagram_path):
    render_er("sqlite:///my.db", diagram_path)

def main(entry: AssignmentEntry) -> Analysis:
    analysis = Analysis()

    if not entry.delivery:
        return analysis
    
    sqlPath = entry.delivery.get_filename("SQL File")
    with open(sqlPath, "r") as f:
        sql = f.read()
    
    generate_db(sql)

    diagram_path = "diagram.png"
    generate_diagram(diagram_path)

    analysis.set_image("Diagram", diagram_path)

    return analysis


if __name__ == "__main__":
    entry = AssignmentEntry.read_from_file()
    analysis = main(entry)
    analysis.write_to_file()