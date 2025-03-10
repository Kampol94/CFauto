import getAuthToken
import bookTraining
import time
from concurrent.futures import ThreadPoolExecutor

kamil = ['kampolaczek@gmail.com', 'yeivpleb', '5791062']
merve = ['ete9707@gmail.com', 'qsfwhueq', '1739102770331']
classId = '33533405'

def book_for_user(email, password, training_id, memberId):
    token = getAuthToken.getAuthToken(email, password)
    attempt_counter = 0
    while not bookTraining.bookTraining(training_id, token, memberId):
        attempt_counter += 1
        print(f"Attempt {attempt_counter} failed for user {email}. Retrying in 30 seconds...")
        time.sleep(10)
    print(f"Booking successful for user {email} after {attempt_counter + 1} attempts.")

with ThreadPoolExecutor(max_workers=2) as executor:
    executor.submit(book_for_user, merve[0], merve[1], classId, merve[2])
    time.sleep(1)
    executor.submit(book_for_user, kamil[0], kamil[1], classId, kamil[2])

