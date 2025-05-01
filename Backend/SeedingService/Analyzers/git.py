from fleximetrics import *
import subprocess
from git import Repo
from collections import defaultdict
from datetime import timedelta
import json
import matplotlib.pyplot as plt

def run_command(command):
    result = subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    if result.stdout:
        print("STDOUT:")
        print(result.stdout)
    if result.stderr:
        print("STDERR:")
        print(result.stderr)
    return result


def clone(url):
    command = [
        "git",
        "clone",
        url,
        "src"
    ]
    run_command(command)


def get_commits_per_author(repo):
    commits_per_author = defaultdict(int)
    for commit in repo.iter_commits(repo.head.reference):
        author = commit.author.name
        commits_per_author[author] += 1
    return commits_per_author


def get_code_edits_per_author(repo):
    additions_per_author = defaultdict(int)
    deletions_per_author = defaultdict(int)
    impact_per_author = defaultdict(int)

    for commit in repo.iter_commits(repo.head.reference):
        author = commit.author.name
        stats = commit.stats.total
        additions_per_author[author] += stats['insertions']
        deletions_per_author[author] += stats['deletions']
        impact_per_author[author] += stats['insertions'] - stats['deletions']

    return additions_per_author, deletions_per_author, impact_per_author


def get_languages(repo):
    repo_path = repo.working_tree_dir
    command = ['cloc', '--json', repo_path]
    result = run_command(command)
    data = json.loads(result.stdout)

    # Remove metadata fields
    for key in ['header', 'SUM']:
        data.pop(key, None)
    return {lang: data[lang]['code'] for lang in data}


def get_commit_date_range(repo):
    commits = list(repo.iter_commits(repo.head.reference))
    first_commit = commits[-1].committed_datetime
    last_commit = commits[0].committed_datetime
    return first_commit, last_commit


def get_commit_history(repo):
    # Weekly commit count from first to last commit
    commits = list(repo.iter_commits(repo.head.reference))

    first_date = commits[-1].committed_datetime.date()
    last_date = commits[0].committed_datetime.date()
    week_counts = defaultdict(int)

    for commit in commits:
        week = commit.committed_datetime.date() - timedelta(days=commit.committed_datetime.weekday())
        week_counts[week] += 1

    # Fill in missing weeks with zero
    current = first_date - timedelta(days=first_date.weekday())
    end = last_date
    history = {}
    while current <= end:
        history[current.isoformat()] = week_counts.get(current, 0)
        current += timedelta(weeks=1)

    commit_history = [value for key, value in sorted(history.items())]

    return commit_history


def main(entry: AssignmentEntry) -> Analysis:
    analysis = Analysis()

    if not entry.delivery:
        return analysis

    url = entry.delivery.get_url("GitHub Repository")
    clone(url)
    repo = Repo("src")

    commits_per_author = get_commits_per_author(repo)
    analysis.set_json("Commits per author", commits_per_author)

    additions_per_author, deletions_per_author, impact_per_author = get_code_edits_per_author(repo)
    analysis.set_json("Additions per author", additions_per_author)
    analysis.set_json("Deletions per author", deletions_per_author)
    analysis.set_json("Impact per author", impact_per_author)

    languages = get_languages(repo)
    analysis.set_json("Languages", languages)

    first_commit, last_commit = get_commit_date_range(repo)
    analysis.set_datetime("First commit", first_commit)
    analysis.set_datetime("Last commit", last_commit)

    commit_history = get_commit_history(repo)
    plt.bar(range(len(commit_history)), commit_history)
    plt.savefig("commit_history.png")
    analysis.set_image("Commit history", "commit_history.png")

    return analysis


if __name__ == "__main__":
    entry = AssignmentEntry.read_from_file()
    analysis = main(entry)
    analysis.write_to_file()