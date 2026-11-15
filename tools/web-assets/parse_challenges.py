#!/usr/bin/env python3
"""Convert the Mars Astronaut Challenges .docx question banks into the web app's JSON.

The source documents are rating-based, not right/wrong: every option carries a 1-5 star
assessment rating, and the 5-star option is the strongest decision. That structure is
preserved here rather than flattened into correct/incorrect.

    python tools/web-assets/parse_challenges.py <open_age.docx> <age_7_12.docx>
"""
import json
import re
import sys
import zipfile
from pathlib import Path
from xml.etree import ElementTree as ET

W = '{http://schemas.openxmlformats.org/wordprocessingml/2006/main}'
ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / '02-Software-Development/Web-Dashboards/astrogateway-web/src/challenges/questions.json'

LEVEL = re.compile(r'^Level\s+(\d+)\s*[–-]\s*(.+)$')
DIFFICULTY = re.compile(r'^Difficulty:\s*([^|]+?)\s*\|\s*Questions\s*(\d+)[–-](\d+)\s*$')
QUESTION = re.compile(r'^(\d{2})\.\s+(.+)$')
RATING = re.compile(r'\((\d)/5\)')
OPTION_KEY = re.compile(r'^[A-D]$')


def paragraphs(path):
    with zipfile.ZipFile(path) as z:
        root = ET.fromstring(z.read('word/document.xml'))
    for p in root.iter(W + 'p'):
        text = ''.join(t.text or '' for t in p.iter(W + 't')).strip()
        if text:
            yield text


def parse(path, track_id, group_label, age_label):
    lines = list(paragraphs(path))
    levels = []
    level = None
    question = None
    pending_key = None
    pending_text = None
    intro = []

    for line in lines:
        if (match := LEVEL.match(line)):
            level = {'n': int(match.group(1)), 'title': match.group(2).strip(),
                     'difficulty': '', 'questions': []}
            levels.append(level)
            question = None
            continue
        if (match := DIFFICULTY.match(line)):
            if level:
                level['difficulty'] = match.group(1).strip()
            continue
        if (match := QUESTION.match(line)):
            if level is None:
                raise SystemExit('Question %s appeared before any Level heading' % match.group(1))
            question = {'id': '%s-%s' % (track_id, match.group(1)), 'n': int(match.group(1)),
                        'title': match.group(2).strip(), 'scenario': '', 'options': []}
            level['questions'].append(question)
            pending_key = pending_text = None
            continue
        # Table header cells repeated per question.
        if line in ('Choice', 'Astronaut action', 'Rating'):
            continue
        if question is None:
            intro.append(line)
            continue
        if not question['scenario']:
            question['scenario'] = line
            continue
        if OPTION_KEY.match(line):
            pending_key = line
            pending_text = None
            continue
        if (match := RATING.search(line)) and pending_key and pending_text:
            question['options'].append({'key': pending_key, 'text': pending_text,
                                        'stars': int(match.group(1))})
            pending_key = pending_text = None
            continue
        if pending_key and pending_text is None:
            pending_text = line
            continue

    questions = [q for lv in levels for q in lv['questions']]
    # Fail loudly rather than shipping a half-parsed bank.
    if len(questions) != 60:
        raise SystemExit('%s: expected 60 questions, parsed %d' % (path.name, len(questions)))
    for q in questions:
        if len(q['options']) < 2:
            raise SystemExit('%s: question %s has %d options' % (path.name, q['id'], len(q['options'])))
        if not any(o['stars'] == 5 for o in q['options']):
            raise SystemExit('%s: question %s has no 5-star option' % (path.name, q['id']))
        if not q['scenario']:
            raise SystemExit('%s: question %s has no scenario text' % (path.name, q['id']))

    return {
        'id': track_id,
        'label': group_label,
        'ageLabel': age_label,
        'source': path.name,
        'levels': levels,
        'questionCount': len(questions),
    }


def main():
    if len(sys.argv) != 3:
        raise SystemExit(__doc__)
    open_age, kids = (Path(sys.argv[1]), Path(sys.argv[2]))
    tracks = [
        parse(kids, 'cadet', 'Cadet', 'Ages 7-12'),
        parse(open_age, 'specialist', 'Specialist', 'Ages 13 and above'),
    ]
    payload = {
        'title': 'Mars Outpost: Astronaut Challenges',
        # Carried from the source documents; both state these explicitly.
        'ratingNote': ('Ratings are preliminary assessment-design ratings, not official NASA '
                       'judgements.'),
        'scienceNote': ('These are simplified educational scenarios, not operational instructions '
                        'for real astronauts. Actual mission decisions require authorised '
                        'procedures, engineering data and expert judgement.'),
        'tracks': tracks,
    }
    OUT.parent.mkdir(parents=True, exist_ok=True)
    OUT.write_text(json.dumps(payload, ensure_ascii=False, indent=1), encoding='utf-8')
    for track in tracks:
        print('%-11s %2d levels  %3d questions  %3d options' % (
            track['id'], len(track['levels']), track['questionCount'],
            sum(len(q['options']) for lv in track['levels'] for q in lv['questions'])))
    print('wrote %s (%.1f KB)' % (OUT.relative_to(ROOT), OUT.stat().st_size / 1024))


if __name__ == '__main__':
    main()
