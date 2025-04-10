from datetime import datetime
import json
from dataclasses import dataclass
from typing import Optional, Any, List, Dict
from dataclass_wizard import JSONWizard
import mimetypes


@dataclass
class Student(JSONWizard):
    student_id: str
    name: str
    email: str


@dataclass
class Team(JSONWizard):
    team_id: str
    team_nr: int
    students: List[Student]


@dataclass
class Delivery(JSONWizard):
    fields: Dict[str, Any]

    def get_str(self, name: str) -> str:
        return str(self.fields[name])

    def get_int(self, name: str) -> int:
        return int(self.fields[name])

    def get_float(self, name: str) -> float:
        return float(self.fields[name])

    def get_bool(self, name: str) -> bool:
        return bool(self.fields[name])

    def get_url(self, name: str) -> str:
        return str(self.fields[name])

    def get_json(self, name: str) -> Any:
        return json.loads(self.fields[name])

    def get_filename(self, name: str) -> str:
        return str(self.fields[name])

    def get_str_list(self, name: str) -> List[str]:
        return [str(f) for f in self.fields[name]]

    def get_int_list(self, name: str) -> List[int]:
        return [int(f) for f in self.fields[name]]

    def get_float_list(self, name: str) -> List[float]:
        return [float(f) for f in self.fields[name]]

    def get_bool_list(self, name: str) -> List[bool]:
        return [bool(f) for f in self.fields[name]]

    def get_url_list(self, name: str) -> List[str]:
        return [str(f) for f in self.fields[name]]

    def get_json_list(self, name: str) -> List[Any]:
        return [json.loads(f) for f in self.fields[name]]


@dataclass
class AssignmentEntry(JSONWizard):
    student: Optional[Student] = None
    team: Optional[Team] = None
    delivery: Optional[Delivery] = None

    @staticmethod
    def read_from_file():
        with open("input.json", "r") as file:
            input_json = file.read()
        print(f"Input: {input_json}")
        return AssignmentEntry.from_json(input_json)


class Analysis:
    def write_to_file(self):
        analysis_json = json.dumps(self.__dict__, sort_keys=True, indent=2)
        print(f"Output: {analysis_json}")
        with open("output.json", "w") as file:
            file.write(analysis_json)

    def set_str(self, name: str, value: str):
        self.__dict__[name] = {"Type": "String", "Value": value}

    def set_int(self, name: str, value: int):
        self.__dict__[name] = {"Type": "Integer", "Value": value}

    def set_float(self, name: str, value: float):
        self.__dict__[name] = {"Type": "Float", "Value": value}

    def set_bool(self, name: str, value: bool):
        self.__dict__[name] = {"Type": "Boolean", "Value": value}

    def set_range(self, name: str, value: int, max_value: int):
        self.__dict__[name] = {
            "Type": "Range",
            "Value": {"Value": value, "Max": max_value},
        }

    def set_datetime(self, name: str, value: datetime):
        self.__dict__[name] = {"Type": "DateTime", "Value": value.isoformat()}

    def set_url(self, name: str, value: str):
        self.__dict__[name] = {"Type": "URL", "Value": value}

    def set_json(self, name: str, value: Any):
        self.__dict__[name] = {
            "Type": "Json",
            "Value": json.dumps(value, sort_keys=True, indent=2),
        }

    def set_str_list(self, name: str, values: List[str]):
        self.__dict__[name] = {"Type": "List", "SubType": "String", "Value": values}

    def set_int_list(self, name: str, values: List[int]):
        self.__dict__[name] = {"Type": "List", "SubType": "Integer", "Value": values}

    def set_float_list(self, name: str, values: List[float]):
        self.__dict__[name] = {"Type": "List", "SubType": "Float", "Value": values}

    def set_bool_list(self, name: str, values: List[bool]):
        self.__dict__[name] = {"Type": "List", "SubType": "Boolean", "Value": values}

    def set_range_list(self, name: str, values: List[int], max_value: int):
        self.__dict__[name] = {
            "Type": "List",
            "SubType": "Range",
            "Value": [{"Value": value, "Max": max_value} for value in values],
        }

    def set_datetime_list(self, name: str, values: List[datetime]):
        self.__dict__[name] = {
            "Type": "List",
            "SubType": "DateTime",
            "Value": [value.isoformat() for value in values],
        }

    def set_url_list(self, name: str, values: List[str]):
        self.__dict__[name] = {"Type": "List", "SubType": "URL", "Value": values}

    def set_json_list(self, name: str, values: List[Any]):
        self.__dict__[name] = {
            "Type": "List",
            "SubType": "Json",
            "Value": [json.dumps(value, sort_keys=True, indent=2) for value in values],
        }

    def set_filename(self, name: str, value: str):
        (content_type, _) = mimetypes.guess_type(value)
        self.__dict__[name] = {
            "Type": "File",
            "Value": {"FileName": value, "ContentType": content_type},
        }
