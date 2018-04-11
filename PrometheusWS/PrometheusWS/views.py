"""
Routes and views for the flask application.
"""

from datetime import datetime
from flask import render_template
from flask import Flask, request
from PrometheusWS import app
import json, os
import PrometheusWS.scoringService

@app.route('/')
@app.route('/home')
def home():
    if request.method == 'POST':
        title = 'POST'
    else:
        title = 'GET'
    return render_template(
        'index.html',
        title=title,
        year=datetime.now().year,
    )

@app.route('/contact')
def contact():
    """Renders the contact page."""
    return render_template(
        'index.html',
        title=title,
        year=datetime.now().year,
    )

@app.route('/about')
def about():
    """Renders the about page."""
    return render_template(
        'about.html',
        title='About',
        year=datetime.now().year,
        message='Your application description page.'
    )

@app.route('/score', methods=['POST', 'GET'])
def score():
    if request.method == 'POST':
        for key, file in request.files.items():
            extension = os.path.splitext(file.filename)[1]
            f_name = 'imgUploaded' + extension
            fullpath = os.path.join(os.path.dirname(os.path.abspath(__file__)), f_name)
            file.save(fullpath)

            results = PrometheusWS.scoringService.run(fullpath)

            return results;
    else:
        return render_template(
            'index.html',
            title=title,
            year=datetime.now().year,
        )