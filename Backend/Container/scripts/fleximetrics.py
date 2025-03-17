import json
from dataclasses import dataclass
from typing import List, Dict, Any


@dataclass
class Student:
    student_id: str
    name: str
    email: str


@dataclass
class Team:
    team_id: str
    team_nr: int
    students: List[Student]


@dataclass
class Delivery:
    fields: Dict[str, Any]

    def get_str(self, name: str) -> str:
        return str(self.fields[name])

    def get_int(self, name: str) -> int:
        return int(self.fields[name])


@dataclass
class AssignmentEntry:
    student: Student | None
    team: Team | None
    delivery: Delivery | None

    def read_from_file():
        with open("input.json", "r") as file:
            input_json = file.read()
        print(f"Input: {input_json}")
        return AssignmentEntry(**json.loads(input_json))


class Analysis:
    def write_to_file(self):
        analysis_json = json.dumps(self.__dict__, sort_keys=True, indent=2)
        print(f"Output: {analysis_json}")
        with open("output.json", "w") as file:
            file.write(analysis_json)

    def set_str(self, name: str, value: str):
        self.__dict__[name] = {"Type": "String", "Value": str(value)}

    def set_int(self, name: str, value: int):
        self.__dict__[name] = {"Type": "Int", "Value": int(value)}
