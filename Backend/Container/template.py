from fleximetrics import *


def main(delivery: Delivery) -> Analysis:
    analysis = Analysis()

    analysis.set_int("Num files", 5)
    analysis.set_str("Title", "Weather app")

    return analysis


if __name__ == "__main__":
    delivery = Delivery.read_from_file()
    analysis = main(delivery)
    analysis.write_to_file()