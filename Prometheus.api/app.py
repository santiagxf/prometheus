"""
The flask application package. NO CHANGES SHOULD BE MADE HERE
"""

from flask import Flask

app = Flask(__name__)

print(" * Flask application is about to start now")
import routes
