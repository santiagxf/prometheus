"""
Routes and views for the flask application.
"""

import json, os
import service.scoring

from app import app
from datetime import datetime
from flask import render_template
from flask import Flask, request, send_from_directory

@app.route('/static/<path:path>')
def send_js(path):
    return send_from_directory('static', path)

@app.route('/')
@app.route('/help')
def help():
    return render_template(
        'index.html',
        title='Prometheus web api',
        year=datetime.now().year,
    )

@app.route('/score', methods=['POST', 'GET'])
def score():
    if request.method == 'POST':
        scoringService = service.scoring.scorer()

        allResults = []

        for key, file in request.files.items():
            extension = os.path.splitext(file.filename)[1]
            scoringId = service.scoring.scorer.generateScoringId()
            fullpath = os.path.join(os.path.dirname(os.path.abspath(__file__)), r'./imagepool/' + scoringId + extension)
            file.save(fullpath)

            allResults.append(scoringService.run(fullpath, file.filename, scoringId))

        return str(json.dumps(allResults))

    else:
        return help()

@app.route('/test')
def test():
    f_name = r'./imagepool/' + 'imgUploaded.jpg'
    fullpath = os.path.join(os.path.dirname(os.path.abspath(__file__)), f_name)

    # initialize
    scoringService = service.scoring.scorer()
    results = scoringService.run(fullpath, 'imgUploaded.jpg')

    return results

@app.route('/supervise/<supervisedLabel>/<scoringId>', methods=['POST'])
def supervise(supervisedLabel, scoringId):
    if (supervisedLabel in ['negative', 'positive']):
        f_name = scoringId + '.jpg'
        l_name = r'./imagepool/' + supervisedLabel  + r'/' + f_name

        unlabeledPath = os.path.join(os.path.dirname(os.path.abspath(__file__)), r'./imagepool/' + f_name)
        labeledPath = os.path.join(os.path.dirname(os.path.abspath(__file__)), l_name)

        os.rename(unlabeledPath, labeledPath)

        return str(json.dumps(supervisedLabel))
