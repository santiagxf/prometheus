"""
The flask application package.
"""

from flask import Flask

app = Flask(__name__)

print(" * Flask application is about to start now")
import routes
