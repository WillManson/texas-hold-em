# texas-hold-em
I designed and built this program in 2014 to allow a user to play Texas Hold 'Em poker against AI opponents. The AI opponents make decisions according to probabilities of winning/losing that they compute. The user can select the difficulty of the opponents: making the opponents harder means that the computer will find the probabilities of obtaining different hands with greater accuracy. This is done by increasing the number of samples taken in the Monte Carlo method used to estimate the probabilities. Also, making the opponents easier means that they will make 'mistakes' with greater probability (e.g. they will chose to call the bet when the probability dictates they should fold, etc.).

Important methods:
* AIPlayer.performGameplayDecision
* HandEvaluator.findProbabilities

I have written extensive documentation for this program, which can be found in this repository.
