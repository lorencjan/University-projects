import hexutil
from json.decoder import JSONDecodeError
import logging
from PyQt5.QtWidgets import QWidget, QGridLayout, QPushButton
from PyQt5.QtGui import QPainter, QColor, QPolygon, QPen, QFont
from PyQt5.QtCore import QPoint, Qt, QRectF, QTimer


# dirty hack to limit the number of transfers per turn even for humans
# The limit needs to be set by user of the module (client script)
MAX_TRANSFERS_PER_TURN = None
# The running counter is handled by MainWindow.mousePressEvent (increments) and ClientUI.handle_end_turn_button (clearing)
nb_transfers_this_turn = 0


def player_color(player_name):
    """Return color of a player given his name
    """
    return {
        1: (0, 255, 0),
        2: (0, 0, 255),
        3: (255, 0, 0),
        4: (255, 255, 0),
        5: (0, 255, 255),
        6: (255, 0, 255),
        7: (224, 224, 224),
        8: (153, 153, 255)
    }[player_name]


class MainWindow(QWidget):
    """Main window of the GUI containing the game board
    """
    def __init__(self, game, area_text_fn=lambda area: str(area.get_dice())):
        """
        Parameters
        ----------
        game : Game
        """
        self.logger = logging.getLogger('GUI')
        super(MainWindow, self).__init__()
        self.qp = QPainter()

        self.game = game
        self.board = game.board
        self.areas_mapping = {}
        for i, area in self.board.areas.items():
            for h in area.get_hexes():
                self.areas_mapping[h] = i

        self.font = QFont('Helvetica', 16)
        self.pen = QPen()
        self.pen.setWidth(2)

        self.activated_area_name = None
        self.area_text_fn = area_text_fn

    def paintEvent(self, event):
        self.qp.begin(self)
        self.draw_areas()
        self.qp.end()

    def set_area_text_fn(self, area_text_fn):
        self.area_text_fn = area_text_fn

    def draw_areas(self):
        """Draw areas in the game board
        """
        if self.game.draw_battle:
            self.game.draw_battle = False
        size = self.size()
        x = size.width()
        y = size.height()

        hexgrid = hexutil.HexGrid(10)

        self.qp.setPen(Qt.NoPen)
        self.qp.translate(x // 2, y // 2)

        for k, area in self.board.areas.items():
            lines = []
            first_hex = True

            color = player_color(area.get_owner_name())
            if self.activated_area_name == int(k):
                color = (170 + color[0] // 3, 170 + color[1] // 3, 170 + color[2] // 3)
            self.qp.setBrush(QColor(*color))
            self.qp.setPen(Qt.NoPen)
            for h in area.get_hexes():
                polygon = QPolygon([QPoint(*corner) for corner in hexgrid.corners(h)])
                self.qp.drawPolygon(polygon)

                if first_hex:
                    self.qp.save()
                    rect = QRectF(*hexgrid.bounding_box(h))
                    self.qp.setBrush(QColor(0, 0, 0))
                    self.qp.setPen(self.pen)
                    self.qp.setFont(self.font)
                    self.qp.setRenderHint(QPainter.TextAntialiasing)

                    self.qp.drawText(rect, Qt.AlignCenter, self.area_text_fn(area))
                    first_hex = False
                    self.qp.restore()

                for n in h.neighbours():
                    if n not in area.get_hexes():
                        line = []
                        for corner in hexgrid.corners(h):
                            if corner in hexgrid.corners(n):
                                line.append(corner)
                        lines.append(line)

            self.qp.save()
            pen = QPen()
            pen.setWidth(3)
            self.qp.setPen(pen)
            self.qp.setBrush(QColor())
            self.qp.setRenderHint(QPainter.Antialiasing)
            for line in lines:
                self.qp.drawLine(line[0][0], line[0][1], line[1][0], line[1][1])
            self.qp.restore()

    def deactivate_area(self):
        self.activated_area_name = None

    def mousePressEvent(self, event):
        hexagon = self.get_hex(event.pos())

        try:
            area = self.board.get_area(self.areas_mapping[hexagon])

            if self.activated_area_name:
                if area.get_name() == self.activated_area_name:
                    self.deactivate_area()
                    self.update()
                elif area.get_name() in self.activated_area.get_adjacent_areas_names():
                    if area.get_owner_name() != self.game.current_player.get_name():
                        self.game.send_message('battle', self.activated_area_name, area.get_name())
                    else:
                        global nb_transfers_this_turn  # dirty hack, see the top of this module
                        if nb_transfers_this_turn < MAX_TRANSFERS_PER_TURN:
                            nb_transfers_this_turn += 1
                            self.game.send_message('transfer', self.activated_area_name, area.get_name())
                        else:
                            print(f'Already did {nb_transfers_this_turn}/{MAX_TRANSFERS_PER_TURN} tranfers allowed per turn')

                    self.deactivate_area()
                    self.update()
            elif (area.get_owner_name() == self.game.player_name and
                  self.game.player_name == self.game.current_player.get_name() and
                  area.can_attack()):
                # area activation
                self.activated_area_name = area.get_name()
                self.activated_area = area
                self.update()
        except KeyError:
            pass

    def get_hex(self, position):
        """Return coordinates of a Hex from the given pixel position
        """
        size = self.size()
        x = size.width()//2
        y = size.height()//2
        hexgrid = hexutil.HexGrid(10)
        return hexgrid.hex_at_coordinate(position.x() - x, position.y() - y)


class Battle(QWidget):
    """Widget for displaying battle results
    """
    def __init__(self, game):
        super(Battle, self).__init__()
        self.game = game

        self.color = QColor(0, 0, 0)
        self.qp = QPainter()
        self.font = QFont('Helvetica', 12)
        self.pen = QPen()
        self.pen.setWidth(2)

    def paintEvent(self, event):
        self.qp.begin(self)
        self.draw_battle(event)
        self.qp.end()

    def draw_battle(self, event):
        """Draw battle results
        """
        rect = event.rect()
        label_rect = QRectF(rect.x(), rect.y(), rect.width(), 25)

        self.qp.setPen(self.color)
        self.qp.setFont(self.font)
        self.qp.drawText(label_rect, Qt.AlignCenter, 'Battle')

        if self.game.battle:
            size = rect.width() // 4

            attacker = QRectF(rect.x() + size, 30 + rect.y(), size - 10, size - 10)
            defender = QRectF(rect.x() + 2 * size, 30 + rect.y(), size - 10, size - 10)

            self.qp.setPen(self.color)
            self.qp.setFont(self.font)

            self.qp.setBrush(QColor(*player_color(self.game.battle['atk_name'])))
            self.qp.drawRect(attacker)
            self.qp.drawText(attacker, Qt.AlignCenter, str(self.game.battle['atk_dice']))

            self.qp.setBrush(QColor(*player_color(self.game.battle['def_name'])))
            self.qp.drawRect(defender)
            self.qp.drawText(defender, Qt.AlignCenter, str(self.game.battle['def_dice']))
            self.game.battle = False

        else:
            size = rect.width() // 4
            self.qp.setBrush(QColor(230, 230, 230))

            attacker = QRectF(rect.x() + size, 30 + rect.y(), size - 10, size - 10)
            deffender = QRectF(rect.x() + 2 * size, 30 + rect.y(), size - 10, size - 10)

            self.qp.drawRect(attacker)
            self.qp.drawRect(deffender)


class Score(QWidget):
    """Widget showing players' scores and dice reserves
    """
    def __init__(self, game):
        super(Score, self).__init__()
        self.game = game

        self.color = QColor(0, 0, 0)
        self.qp = QPainter()
        self.font = QFont('Helvetica', 16)
        self.pen = QPen()
        self.pen.setWidth(2)

    def paintEvent(self, event):
        self.qp.begin(self)
        self.qp.setRenderHint(QPainter.Antialiasing)
        self.draw_scores(event)
        self.qp.end()

    def draw_scores(self, event):
        """Redraw scores and reserves
        """
        rect = event.rect()
        label_rect = QRectF(rect.x(), rect.y(), rect.width(), 25)

        self.qp.setPen(self.color)
        self.qp.setFont(self.font)
        self.qp.drawText(label_rect, Qt.AlignCenter, 'Scores')

        size = rect.width() // 4
        for i, p in self.game.players.items():
            player_score_rect = QRectF(rect.x() + (i-1) % 4*size + 5, 30 + rect.y() + ((i-1)//4) * size,
                                       size - 10, size - 10)
            reserve_rect = QRectF(player_score_rect.x() + 40, player_score_rect.y() + 40, 20, 20)

            self.qp.save()
            if p == self.game.current_player:
                pen = QPen()
                pen.setWidth(5)
                self.qp.setPen(pen)

            self.qp.setBrush(QColor(*player_color(i)))
            self.qp.drawRect(player_score_rect)
            self.qp.restore()

            self.qp.setPen(self.color)
            self.qp.setFont(self.font)
            self.qp.drawText(player_score_rect, Qt.AlignCenter, str(p.get_score()))

            self.qp.setFont(QFont('Helvetica', 8))
            self.qp.drawText(reserve_rect, Qt.AlignCenter, str(p.get_reserve()))


class StatusArea(QWidget):
    """Status area showing current player
    """
    def __init__(self, game):
        super(StatusArea, self).__init__()
        self.game = game

        self.color = QColor(0, 0, 0)
        self.qp = QPainter()
        self.font = QFont('Helvetica', 10)
        self.pen = QPen()
        self.pen.setWidth(2)

    def paintEvent(self, event):
        self.qp.begin(self)
        self.qp.setPen(self.color)
        self.qp.setFont(self.font)
        self.qp.drawText(event.rect(), Qt.AlignCenter, 'Player {0}\'s turn.'.format(self.game.current_player.get_name()))
        self.qp.end()


class ClientUI(QWidget):
    """Dice Wars' graphical user interface
    """
    def __init__(self, game):
        """
        Parameters
        ----------
        game : Game
        """
        super(ClientUI, self).__init__()
        self.logger = logging.getLogger('GUI')
        self.game = game
        self.window_name = 'Dice Wars - Player ' + str(self.game.player_name)
        self.init_ui()

        self.socket_timer = QTimer()
        self.socket_timer.timeout.connect(self.check_socket)
        self.socket_timer.start(10)
        self.game.battle = False
        self.game.draw_battle = False

    def init_ui(self):
        self.resize(1024, 576)
        self.setMinimumSize(1024, 576)
        self.setWindowTitle(self.window_name)

        self.init_layout()
        self.show()

    def init_layout(self):
        grid = QGridLayout()

        self.main_area = MainWindow(self.game)
        self.battle_area = Battle(self.game)
        self.score_area = Score(self.game)
        self.status_area = StatusArea(self.game)
        self.end_turn = QPushButton('End turn')
        self.end_turn.clicked.connect(self.handle_end_turn_button)

        if self.game.player_name == self.game.current_player.get_name():
            self.end_turn.setEnabled(True)
        else:
            self.end_turn.setEnabled(False)

        grid.addWidget(self.main_area, 0, 0, 10, 8)
        grid.addWidget(self.battle_area, 0, 8, 4, 3)
        grid.addWidget(self.score_area, 4, 8, 4, 3)
        grid.addWidget(self.end_turn, 8, 9, 1, 1)
        grid.addWidget(self.status_area, 9, 8, 1, 3)

        self.setLayout(grid)

    def handle_end_turn_button(self):
        global nb_transfers_this_turn  # dirty hack, see the top section of the module
        nb_transfers_this_turn = 0
        self.game.send_message('end_turn')

    def check_socket(self):
        """Check server message queue for incoming messages
        """
        if not self.game.input_queue.empty():
            event = self.game.input_queue.get()
            if not self.handle_server_message(event):
                self.logger.debug('Game has ended.')

    def handle_server_message(self, event):
        """Handle event associated to message from server
        """
        self.game.draw_battle = False

        try:
            msg = event
        except JSONDecodeError as e:
            self.logger.debug(e)
            self.logger.debug('msg = {}'.format(event))
            exit(1)

        if msg['type'] == 'battle':
            self.game.process_battle_msg(msg)

            self.game.draw_battle = True

            atk_data = msg['result']['atk']
            def_data = msg['result']['def']
            atk_name = self.game.board.get_area(str(atk_data['name'])).get_owner_name()
            def_name = self.game.board.get_area(def_data['name']).get_owner_name()

            self.game.battle = {
                'atk_name': atk_name,
                'def_name': def_name,
                'atk_dice': atk_data['pwr'],
                'def_dice': def_data['pwr']
            }

        elif msg['type'] == 'transfer':
            self.game.process_transfer_msg(msg)

        elif msg['type'] == 'end_turn':
            self.game.process_end_turn_msg(msg)
            self.game.battle = False

        elif msg['type'] == 'game_end':
            if msg['winner'] == self.game.player_name:
                print("YOU WIN!")
            else:
                print("Player {} has won".format(msg['winner']))
            self.game.socket.close()
            exit(0)

        self.main_area.update()
        self.battle_area.update()
        self.score_area.update()
        self.status_area.update()

        if self.game.player_name == self.game.current_player.get_name():
            self.end_turn.setEnabled(True)
        else:
            self.end_turn.setEnabled(False)

        return True
