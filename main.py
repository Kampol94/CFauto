import getAuthToken
import bookTraining
import time

token = getAuthToken.getAuthToken('***', '***')
while bookTraining.bookTraining('***', token) == False:
     time.sleep(10)
