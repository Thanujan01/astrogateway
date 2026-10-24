import uvicorn
from main import app
uvicorn.run(app,host='127.0.0.1',port=8000,access_log=False,log_level='critical')
