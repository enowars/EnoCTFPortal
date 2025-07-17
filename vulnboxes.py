import sys
import json
def print_usage():
    print("""
Usage:
    python script.py <range:location> [<range:location> ...]

Description:
    Takes range-to-location mappings and outputs a JSON mapping from each number in the range to the location.

Arguments:
    <range:location>  Format should be start-end:location
                      Example: 0-10:fsn1 11-20:nbg1 30-50:hel1

Output:
    Writes a JSON file called 'output.json' mapping each number in the specified ranges to their corresponding location.
""")
def parse_range_mapping(args):
    mapping = {}
    for arg in args:
        try:
            range_part, location = arg.split(":")
            start, end = map(int, range_part.split("-"))
            for i in range(start, end + 1):
                mapping[str(i)] = location
        except ValueError:
            print(f"Invalid argument format: {arg}. Expected format is start-end:location")
    return mapping

if __name__ == "__main__":
    if len(sys.argv) < 2 or sys.argv[1] in ("-h", "--help"):
        print_usage()
        sys.exit(0)

    args = sys.argv[1:]
    result = parse_range_mapping(args)

    with open("vulnboxes.json", "w") as f:
        json.dump(result, f, indent=4)
    print("JSON file 'vulnoxes.json' generated successfully.")