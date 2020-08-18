DROP DATABASE IF EXISTS agents_seller;
CREATE DATABASE agents_seller DEFAULT CHARACTER SET utf8 COLLATE utf8_polish_ci;
USE agents_seller;

CREATE TABLE IF NOT EXISTS preferences (
  id INTEGER PRIMARY KEY AUTO_INCREMENT,
  name varchar(120) UNIQUE NOT NULL
) ENGINE=InnoDB DEFAULT CHARACTER SET utf8 COLLATE utf8_polish_ci;

INSERT INTO preferences (name) VALUES
('Ile danych byłoby przechowywanych?'),
('Preferowany wygląd laptopa?'),
('Jaka wielkość laptopa?'),
('Do czego głównie laptop byłby używany?'),
('Jak często laptop znajdowałby z daleka od źródła zasilania?'),
('Jak bardzo powinien być wytrzymały?'),
('Jak często będzie używany w nocy?'),
('Czy będzie używany odtwarzacz CD/DVD?');


CREATE TABLE IF NOT EXISTS options (
  id INTEGER PRIMARY KEY AUTO_INCREMENT,
  name varchar(120) UNIQUE NOT NULL
) ENGINE=InnoDB DEFAULT CHARACTER SET utf8 COLLATE utf8_polish_ci;

INSERT INTO options (name) VALUES
('Bardzo Dużo'),
('Dużo'),
('Mało'),

('Ciemny'),
('Jasny'),
('Kolorowy'),

('Duży'),
('Średni'),
('Mały'),

('Biznesowy'),
('Granie w Gry'),
('Multimedia i Przeglądanie Internetu'),
('Prace Graficzne'),

('Często'),
('Sporadycznie'),
('Rzadko'),

('Bardzo'),
('Trochę'),
('Nie Musi'),

('Tak'),
('Nie');


CREATE TABLE IF NOT EXISTS preferencesOptions (
  id INTEGER PRIMARY KEY AUTO_INCREMENT,
  id_preferences INTEGER NOT NULL,
  id_options INTEGER NOT NULL,
  FOREIGN KEY (id_preferences) REFERENCES preferences(id),
  FOREIGN KEY (id_options) REFERENCES options(id)
) ENGINE=InnoDB DEFAULT CHARACTER SET utf8 COLLATE utf8_polish_ci;

INSERT INTO preferencesOptions (id_preferences,id_options) VALUES
(1,1),
(1,2),
(1,3),

(2,4),
(2,5),
(2,6),

(3,7),
(3,8),
(3,9),

(4,10),
(4,11),
(4,12),
(4,13),

(5,14),
(5,15),
(5,16),

(6,17),
(6,18),
(6,19),

(7,14),
(7,15),
(7,16),

(8,20),
(8,21);


CREATE TABLE IF NOT EXISTS items (
  id INTEGER PRIMARY KEY AUTO_INCREMENT,
  name varchar(120) UNIQUE NOT NULL,
  price INTEGER NOT NULL,
  link varchar(255) UNIQUE NOT NULL
) ENGINE=InnoDB DEFAULT CHARACTER SET utf8 COLLATE utf8_polish_ci;

INSERT INTO items (price,name,link) VALUES
(3499,'Dell Vostro 3590','https://www.google.com/search?q=Dell+Vostro+3590'),
(2649,'ASUS X509JA-EJ025R','https://www.google.com/search?q=ASUS+X509JA-EJ025R'),
(9999,'HP Elite Dragonfly','https://www.google.com/search?q=HP+Elite+Dragonfly'),
(10299,'Dell Precision 3541','https://www.google.com/search?q=Dell+Precision+3541'),
(4199,'Dell Inspiron 3793','https://www.google.com/search?q=Dell%20Inspiron%203793'),
(2699,'ASUS TUF Gaming FX505DY','https://www.google.com/search?q=ASUS%20TUF%20Gaming%20FX505DY'),
(4399,'MSI GL65','https://www.google.com/search?q=MSI%20GL65'),
(5799,'ASUS ROG Zephyrus G15','https://www.google.com/search?q=ASUS%20ROG%20Zephyrus%20G15'),
(22999,'Dell Alienware 51m','https://www.google.com/search?q=Dell%20Alienware%2051m'),
(16499,'Acer ConceptD 7','https://www.google.com/search?q=Acer%20ConceptD%207'),
(4999,'ASUS ZenBook Flip UX562FD','https://www.google.com/search?q=ASUS%20ZenBook%20Flip%20UX562FD'),
(10499,'Lenovo ThinkPad P72','https://www.google.com/search?q=Lenovo%20ThinkPad%20P72'),
(4999,'Microsoft Surface Laptop 3 i5','https://www.google.com/search?q=Microsoft%20Surface%20Laptop%203%20i5'),
(7299,'Microsoft Surface Laptop 3 R5','https://www.google.com/search?q=Microsoft%20Surface%20Laptop%203%20R5'),
(16299,'Microsoft Surface Book 2 15','https://www.google.com/search?q=Microsoft%20Surface%20Book%202%2015'),
(4499,'HP Pavilion 15','https://www.google.com/search?q=HP%20Pavilion%2015'),
(2799,'Dell Vostro 3580','https://www.google.com/search?q=Dell%20Vostro%203580'),
(6499,'HP Envy 17','https://www.google.com/search?q=HP%20Envy%2017'),
(4799,'Acer Nitro 5','https://www.google.com/search?q=Acer%20Nitro%205'),
(4599,'Apple MacBook Air','https://www.google.com/search?q=Apple%20MacBook%20Air');


CREATE TABLE IF NOT EXISTS itemsSpecification (
  id INTEGER PRIMARY KEY AUTO_INCREMENT,
  id_items INTEGER NOT NULL,
  id_preferencesOptions INTEGER NOT NULL,
  priority enum('0','1','2','3') NOT NULL,
  FOREIGN KEY (id_items) REFERENCES items(id),
  FOREIGN KEY (id_preferencesOptions) REFERENCES preferencesOptions(id)
) ENGINE=InnoDB DEFAULT CHARACTER SET utf8 COLLATE utf8_polish_ci;

INSERT INTO itemsSpecification (id_items,id_preferencesOptions,priority) VALUES
(1, 1,'0'),
(1, 2,'1'),
(1, 3,'3'),
(1, 4,'3'),
(1, 5,'0'),
(1, 6,'1'),
(1, 7,'1'),
(1, 8,'3'),
(1, 9,'0'),
(1,10,'3'),
(1,11,'1'),
(1,12,'3'),
(1,13,'1'),
(1,14,'2'),
(1,15,'3'),
(1,16,'3'),
(1,17,'1'),
(1,18,'2'),
(1,19,'3'),
(1,20,'1'),
(1,21,'2'),
(1,22,'3'),
(1,23,'0'),
(1,24,'3'),

(2, 1,'0'),
(2, 2,'1'),
(2, 3,'3'),
(2, 4,'2'),
(2, 5,'2'),
(2, 6,'1'),
(2, 7,'1'),
(2, 8,'3'),
(2, 9,'0'),
(2,10,'3'),
(2,11,'1'),
(2,12,'2'),
(2,13,'1'),
(2,14,'3'),
(2,15,'2'),
(2,16,'1'),
(2,17,'1'),
(2,18,'2'),
(2,19,'3'),
(2,20,'1'),
(2,21,'2'),
(2,22,'3'),
(2,23,'0'),
(2,24,'3'),

(3, 1,'3'),
(3, 2,'2'),
(3, 3,'1'),
(3, 4,'2'),
(3, 5,'1'),
(3, 6,'2'),
(3, 7,'3'),
(3, 8,'1'),
(3, 9,'0'),
(3,10,'3'),
(3,11,'1'),
(3,12,'3'),
(3,13,'1'),
(3,14,'3'),
(3,15,'2'),
(3,16,'1'),
(3,17,'2'),
(3,18,'3'),
(3,19,'2'),
(3,20,'3'),
(3,21,'2'),
(3,22,'2'),
(3,23,'0'),
(3,24,'3'),

(4, 1,'3'),
(4, 2,'2'),
(4, 3,'1'),
(4, 4,'3'),
(4, 5,'0'),
(4, 6,'1'),
(4, 7,'2'),
(4, 8,'3'),
(4, 9,'2'),
(4,10,'3'),
(4,11,'2'),
(4,12,'3'),
(4,13,'2'),
(4,14,'3'),
(4,15,'2'),
(4,16,'1'),
(4,17,'1'),
(4,18,'2'),
(4,19,'3'),
(4,20,'1'),
(4,21,'2'),
(4,22,'3'),
(4,23,'0'),
(4,24,'3'),

(5, 1,'2'),
(5, 2,'3'),
(5, 3,'2'),
(5, 4,'1'),
(5, 5,'3'),
(5, 6,'1'),
(5, 7,'0'),
(5, 8,'1'),
(5, 9,'3'),
(5,10,'3'),
(5,11,'2'),
(5,12,'3'),
(5,13,'2'),
(5,14,'2'),
(5,15,'3'),
(5,16,'3'),
(5,17,'1'),
(5,18,'2'),
(5,19,'3'),
(5,20,'1'),
(5,21,'2'),
(5,22,'3'),
(5,23,'3'),
(5,24,'0'),

(6, 1,'0'),
(6, 2,'1'),
(6, 3,'2'),
(6, 4,'3'),
(6, 5,'0'),
(6, 6,'1'),
(6, 7,'1'),
(6, 8,'3'),
(6, 9,'0'),
(6,10,'2'),
(6,11,'3'),
(6,12,'2'),
(6,13,'2'),
(6,14,'2'),
(6,15,'3'),
(6,16,'2'),
(6,17,'1'),
(6,18,'2'),
(6,19,'3'),
(6,20,'3'),
(6,21,'3'),
(6,22,'2'),
(6,23,'0'),
(6,24,'3'),

(7, 1,'0'),
(7, 2,'0'),
(7, 3,'3'),
(7, 4,'3'),
(7, 5,'0'),
(7, 6,'1'),
(7, 7,'2'),
(7, 8,'3'),
(7, 9,'2'),
(7,10,'2'),
(7,11,'3'),
(7,12,'2'),
(7,13,'2'),
(7,14,'3'),
(7,15,'2'),
(7,16,'2'),
(7,17,'2'),
(7,18,'3'),
(7,19,'2'),
(7,20,'3'),
(7,21,'3'),
(7,22,'2'),
(7,23,'0'),
(7,24,'3'),

(8, 1,'1'),
(8, 2,'2'),
(8, 3,'2'),
(8, 4,'3'),
(8, 5,'0'),
(8, 6,'1'),
(8, 7,'1'),
(8, 8,'3'),
(8, 9,'0'),
(8,10,'1'),
(8,11,'3'),
(8,12,'2'),
(8,13,'2'),
(8,14,'2'),
(8,15,'2'),
(8,16,'2'),
(8,17,'2'),
(8,18,'3'),
(8,19,'2'),
(8,20,'3'),
(8,21,'3'),
(8,22,'2'),
(8,23,'0'),
(8,24,'3'),

(9, 1,'3'),
(9, 2,'2'),
(9, 3,'0'),
(9, 4,'2'),
(9, 5,'1'),
(9, 6,'1'),
(9, 7,'0'),
(9, 8,'1'),
(9, 9,'3'),
(9,10,'1'),
(9,11,'3'),
(9,12,'1'),
(9,13,'3'),
(9,14,'1'),
(9,15,'2'),
(9,16,'3'),
(9,17,'3'),
(9,18,'2'),
(9,19,'1'),
(9,20,'3'),
(9,21,'3'),
(9,22,'1'),
(9,23,'0'),
(9,24,'3'),

(10, 1,'0'),
(10, 2,'2'),
(10, 3,'2'),
(10, 4,'0'),
(10, 5,'3'),
(10, 6,'2'),
(10, 7,'1'),
(10, 8,'3'),
(10, 9,'0'),
(10,10,'1'),
(10,11,'3'),
(10,12,'1'),
(10,13,'3'),
(10,14,'3'),
(10,15,'2'),
(10,16,'1'),
(10,17,'3'),
(10,18,'2'),
(10,19,'1'),
(10,20,'2'),
(10,21,'3'),
(10,22,'2'),
(10,23,'0'),
(10,24,'3'),

(11, 1,'0'),
(11, 2,'1'),
(11, 3,'2'),
(11, 4,'2'),
(11, 5,'2'),
(11, 6,'1'),
(11, 7,'1'),
(11, 8,'3'),
(11, 9,'0'),
(11,10,'1'),
(11,11,'2'),
(11,12,'1'),
(11,13,'2'),
(11,14,'3'),
(11,15,'3'),
(11,16,'1'),
(11,17,'3'),
(11,18,'2'),
(11,19,'1'),
(11,20,'3'),
(11,21,'2'),
(11,22,'1'),
(11,23,'0'),
(11,24,'3'),

(12, 1,'0'),
(12, 2,'1'),
(12, 3,'2'),
(12, 4,'3'),
(12, 5,'0'),
(12, 6,'1'),
(12, 7,'0'),
(12, 8,'1'),
(12, 9,'3'),
(12,10,'1'),
(12,11,'2'),
(12,12,'1'),
(12,13,'2'),
(12,14,'3'),
(12,15,'2'),
(12,16,'1'),
(12,17,'3'),
(12,18,'3'),
(12,19,'1'),
(12,20,'3'),
(12,21,'3'),
(12,22,'1'),
(12,23,'0'),
(12,24,'3'),

(13, 1,'0'),
(13, 2,'0'),
(13, 3,'3'),
(13, 4,'1'),
(13, 5,'2'),
(13, 6,'1'),
(13, 7,'3'),
(13, 8,'1'),
(13, 9,'0'),
(13,10,'2'),
(13,11,'1'),
(13,12,'2'),
(13,13,'3'),
(13,14,'3'),
(13,15,'2'),
(13,16,'1'),
(13,17,'2'),
(13,18,'3'),
(13,19,'2'),
(13,20,'2'),
(13,21,'2'),
(13,22,'3'),
(13,23,'0'),
(13,24,'3'),

(14, 1,'0'),
(14, 2,'0'),
(14, 3,'3'),
(14, 4,'2'),
(14, 5,'1'),
(14, 6,'1'),
(14, 7,'1'),
(14, 8,'3'),
(14, 9,'0'),
(14,10,'2'),
(14,11,'1'),
(14,12,'2'),
(14,13,'3'),
(14,14,'3'),
(14,15,'2'),
(14,16,'0'),
(14,17,'2'),
(14,18,'3'),
(14,19,'2'),
(14,20,'2'),
(14,21,'2'),
(14,22,'3'),
(14,23,'0'),
(14,24,'3'),

(15, 1,'0'),
(15, 2,'2'),
(15, 3,'1'),
(15, 4,'2'),
(15, 5,'2'),
(15, 6,'1'),
(15, 7,'1'),
(15, 8,'3'),
(15, 9,'0'),
(15,10,'2'),
(15,11,'2'),
(15,12,'2'),
(15,13,'3'),
(15,14,'3'),
(15,15,'2'),
(15,16,'0'),
(15,17,'3'),
(15,18,'2'),
(15,19,'0'),
(15,20,'2'),
(15,21,'2'),
(15,22,'3'),
(15,23,'0'),
(15,24,'3'),

(16, 1,'0'),
(16, 2,'2'),
(16, 3,'1'),
(16, 4,'1'),
(16, 5,'2'),
(16, 6,'3'),
(16, 7,'1'),
(16, 8,'3'),
(16, 9,'0'),
(16,10,'3'),
(16,11,'2'),
(16,12,'3'),
(16,13,'1'),
(16,14,'1'),
(16,15,'2'),
(16,16,'3'),
(16,17,'2'),
(16,18,'3'),
(16,19,'0'),
(16,20,'3'),
(16,21,'2'),
(16,22,'1'),
(16,23,'0'),
(16,24,'3'),

(17, 1,'0'),
(17, 2,'0'),
(17, 3,'3'),
(17, 4,'3'),
(17, 5,'0'),
(17, 6,'1'),
(17, 7,'2'),
(17, 8,'3'),
(17, 9,'2'),
(17,10,'3'),
(17,11,'1'),
(17,12,'3'),
(17,13,'1'),
(17,14,'1'),
(17,15,'2'),
(17,16,'3'),
(17,17,'0'),
(17,18,'1'),
(17,19,'3'),
(17,20,'1'),
(17,21,'2'),
(17,22,'3'),
(17,23,'3'),
(17,24,'0'),

(18, 1,'1'),
(18, 2,'3'),
(18, 3,'1'),
(18, 4,'1'),
(18, 5,'2'),
(18, 6,'2'),
(18, 7,'0'),
(18, 8,'1'),
(18, 9,'3'),
(18,10,'3'),
(18,11,'2'),
(18,12,'3'),
(18,13,'2'),
(18,14,'2'),
(18,15,'3'),
(18,16,'2'),
(18,17,'2'),
(18,18,'3'),
(18,19,'1'),
(18,20,'3'),
(18,21,'2'),
(18,22,'1'),
(18,23,'3'),
(18,24,'0'),

(19, 1,'1'),
(19, 2,'2'),
(19, 3,'3'),
(19, 4,'3'),
(19, 5,'0'),
(19, 6,'1'),
(19, 7,'1'),
(19, 8,'3'),
(19, 9,'1'),
(19,10,'2'),
(19,11,'3'),
(19,12,'3'),
(19,13,'3'),
(19,14,'1'),
(19,15,'2'),
(19,16,'3'),
(19,17,'0'),
(19,18,'1'),
(19,19,'3'),
(19,20,'3'),
(19,21,'2'),
(19,22,'1'),
(19,23,'0'),
(19,24,'3'),

(20, 1,'0'),
(20, 2,'1'),
(20, 3,'3'),
(20, 4,'2'),
(20, 5,'2'),
(20, 6,'1'),
(20, 7,'3'),
(20, 8,'1'),
(20, 9,'0'),
(20,10,'3'),
(20,11,'1'),
(20,12,'3'),
(20,13,'1'),
(20,14,'3'),
(20,15,'2'),
(20,16,'1'),
(20,17,'2'),
(20,18,'3'),
(20,19,'3'),
(20,20,'3'),
(20,21,'2'),
(20,22,'1'),
(20,23,'0'),
(20,24,'3');




DROP DATABASE IF EXISTS agents_customer;
CREATE DATABASE agents_customer DEFAULT CHARACTER SET utf8 COLLATE utf8_polish_ci;
USE agents_customer;