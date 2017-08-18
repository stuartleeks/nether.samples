(function () {
    var
        config = {
            netherBaseUrl: 'http://localhost:5000', // TODO get from env

            providers: nether.player.identity.providers.guest,
            providerConfig: [{
                provider: nether.player.identity.providers.guest,
                netherClientId: 'devclient',// TODO get from env
                netherClientSecret: 'devsecret', // TODO get from env
            }]
        },
        eltLoadingDiv = document.getElementById('loading'),
        eltPlayDiv = document.getElementById('play'),
        eltPlayGamertag = document.getElementById('gamertag'),
        eltPlayCards = document.getElementById('cards'),
        eltPlayMessage = document.getElementById('play-message'),
        eltPlayHigher = document.getElementById('play-higher'),
        eltPlayLower = document.getElementById('play-lower'),
        eltSetGamertagDiv = document.getElementById('set-gamertag'),
        eltNewGamertag = document.getElementById('new-gamertag'),
        eltSetGamertagMessage = document.getElementById('set-gamertag-message'),
        eltSetGamertagButton = document.getElementById('set-gamertag-button'),
        eltLeaderboardDiv = document.getElementById('leaderboard'),
        eltLeaderboardScores = document.getElementById('leaderboard-scores'),
        eltLeaderboardMessage1 = document.getElementById('leaderboard-message1'),
        eltLeaderboardMessage2 = document.getElementById('leaderboard-message2'),
        eltLeaderboardPlayAgainButton = document.getElementById('leaderboard-play-again'),
        gamertag = null,
        deck = [],
        playedCards = [],
        playedOption = null,
        playOptions = { 'higher': 'higher', 'lower': 'lower' },
        lastScore = null;

    function logMessage(message) {
        console.log(message);
    }

    function logIn() {
        logMessage('Calling guest login');
        nether.player.identity.guestLogin(function (loggedIn) {
            if (!loggedIn) {
                logMessage("failed to log in"); // TODO - how to get errors?
                return;
            }
            logMessage('Getting player information');
            nether.player.getPlayer(function (player) {
                logMessage(JSON.stringify(player));
                if (player.gamertag === null) {
                    // prompt for a gamertag
                    eltLoadingDiv.classList.remove('active');
                    eltSetGamertagDiv.classList.add('active');
                } else {
                    // start the game
                    gamertag = player.gamertag;
                    startGame();
                }
            });
        });
    }
    function startGame() {
        eltLeaderboardDiv.classList.remove('active');
        eltLoadingDiv.classList.remove('active');
        eltPlayDiv.classList.add('active');
        eltPlayHigher.disabled = false;
        eltPlayHigher.classList.remove('disabled');
        eltPlayLower.disabled = false;
        eltPlayLower.classList.remove('disabled');

        eltPlayCards.innerHTML = '';
        eltPlayGamertag.innerText = gamertag;
        playedCards = [];
        addCard();
    }
    function showLeaderboard() {
        eltLeaderboardMessage1.innerText = '';
        eltLeaderboardMessage2.innerText = '';
        eltPlayDiv.classList.remove('active');
        eltLeaderboardDiv.classList.add('active');

        nether.leaderboard.getLeaderboard('5_AroundMe', function leaderboardCallback(status, leaderboard) {
            // {"entries":[{"gamertag":"test4","score":3,"rank":1,"isCurrentPlayer":true}],"currentPlayer":{"gamertag":"test4","score":3,"rank":1}}
            if (status) {
                var scores = leaderboard.entries;
                var leaderboardHTML = "";
                for (var index = 0; index < scores.length; index++) {
                    var score = scores[index];
                    var selected = '';
                    if (score.gamertag === gamertag) {
                        selected = ' current-player';
                    }
                    leaderboardHTML += '<div class="score-row' + selected + '"><span class="rank">' + score.rank + '</span><span class="gamertag">' + score.gamertag + '</span><span class="score">' + score.score + '</span></div>';
                }
                eltLeaderboardScores.innerHTML = leaderboardHTML;
                var playerHighScore = leaderboard.currentPlayer.score;
                var playerRank = leaderboard.currentPlayer.rank;

                eltLeaderboardMessage1.innerText = 'Sorry, you lose! better luck next time. You scored ' + lastScore + ' ( rank ' + playerRank + ')';
                if (lastScore === playerHighScore) {
                    eltLeaderboardMessage2.innerText = 'Congratulations - new personal best!';
                } else {
                    eltLeaderboardMessage2.innerText = '';
                }
            }
        });
    }

    function addCard() {
        function newDeck() {
            var suits = ['clubs', 'diamonds', 'hearts', 'spades'];
            var valueNames = ['two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine', 'ten', 'jack', 'queen', 'king', 'ace'];
            var cards = [];
            for (var suit = 0; suit < suits.length; suit++) {
                for (var value = 0; value < valueNames.length; value++) {
                    cards.push({ suit: suits[suit], value: value, valueName: valueNames[value] });
                }
            }
            // shuffle
            for (var index = 0; index < 500; index++) {
                var shuffleIndex1 = Math.floor(cards.length * Math.random()),
                    shuffleIndex2 = Math.floor(cards.length * Math.random()),
                    tempCard = cards[shuffleIndex1];
                cards[shuffleIndex1] = cards[shuffleIndex2];
                cards[shuffleIndex2] = tempCard;
            }
            return cards;
        }
        if (deck.length === 0) {
            deck = newDeck();
        }
        var card = deck.pop();
        playedCards.push(card);
        eltPlayCards.innerHTML += '<span class="card"><img src="img/' + card.valueName + '_of_' + card.suit + '.png" /></span>';

        var a = (card.valueName[0] == 'a' || card.valueName[0] == 'e') ? 'an ' : 'a ';
        eltPlayMessage.innerText = 'Higher or lower than ' + a + card.valueName + '?';
        return card;
    }

    function play(option) {
        eltPlayHigher.disabled = true;
        eltPlayHigher.classList.add('disabled');
        eltPlayLower.disabled = true;
        eltPlayLower.classList.add('disabled');

        var
            card = playedCards[playedCards.length - 1],
            a = (card.valueName[0] == 'a' || card.valueName[0] == 'e') ? 'an ' : 'a ';

        eltPlayMessage.innerHTML = '<span class="animate-pulse-fast">You selected ' + option + ' than ' + a + card.valueName + '...</span>';

        setTimeout(function () {
            var newCard = addCard(),
                continuePlaying = false;
            if (option === playOptions.higher) {
                if (newCard.value > card.value)
                    continuePlaying = true;
            } else {
                if (newCard.value < card.value)
                    continuePlaying = true;
            }
            if (continuePlaying) {
                eltPlayHigher.disabled = false;
                eltPlayHigher.classList.remove('disabled');
                eltPlayLower.disabled = false;
                eltPlayLower.classList.remove('disabled');
            } else {
                lastScore = playedCards.length - 2;
                eltPlayMessage.innerText = 'Sorry, you lose! better luck next time. You scored ' + lastScore;
                setTimeout(function () {
                    nether.scores.addScore(lastScore, function () {
                        showLeaderboard();
                    });
                }, 1500);
            }
        }, 1000);
    }

    eltPlayHigher.onclick = function onPlayHigherClick() {
        play(playOptions.higher);
    };
    eltPlayLower.onclick = function onPlayLowerClick() {
        play(playOptions.lower);
    };

    eltLeaderboardPlayAgainButton.onclick = function () {
        startGame();
    };
    eltSetGamertagButton.onclick = function onSetGamertagClick() {
        eltSetGamertagMessage.innerText = '';

        var gamertag = eltNewGamertag.value;
        logMessage('Setting gamertag to ' + gamertag);

        // not currently caching anything, so get the player info to reset it!
        nether.player.getPlayer(function (player) {
            nether.player.setPlayer(player.country, gamertag, player.customTag, function (status, res) {
                if (status) {
                    // need to refresh the token with the gamertag
                    eltSetGamertagDiv.classList.remove('active');
                    eltLoadingDiv.classList.add('active');
                    logIn();
                } else {
                    logMessage(res);
                    eltSetGamertagMessage.innerText = res;
                }
            });
        });
    };

    // initialise nether
    logMessage('Initialising nether sdk');
    nether.init(config);
    logIn();


})();