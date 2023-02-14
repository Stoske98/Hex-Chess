-- phpMyAdmin SQL Dump
-- version 5.1.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Feb 09, 2023 at 03:50 PM
-- Server version: 10.4.18-MariaDB
-- PHP Version: 7.3.27

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `hexgame`
--

-- --------------------------------------------------------

--
-- Table structure for table `ability`
--

CREATE TABLE `ability` (
  `ability_id` int(11) NOT NULL,
  `global_id` int(11) NOT NULL,
  `unit_id` int(11) NOT NULL,
  `ability_type` int(11) NOT NULL,
  `max_cooldown` int(11) NOT NULL,
  `current_cooldown` int(11) NOT NULL,
  `ability_range` int(11) NOT NULL,
  `quantity` int(11) NOT NULL,
  `cc_time` int(11) NOT NULL,
  `ability_name` varchar(20) COLLATE utf8_bin NOT NULL,
  `special` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `accounts`
--

CREATE TABLE `accounts` (
  `account_id` int(11) NOT NULL,
  `device_id` varchar(100) COLLATE utf8_bin NOT NULL,
  `nickname` varchar(20) COLLATE utf8_bin NOT NULL,
  `rank` int(11) NOT NULL DEFAULT 500,
  `selected_class` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `cc`
--

CREATE TABLE `cc` (
  `cc_id` int(11) NOT NULL,
  `match_id` int(11) NOT NULL,
  `from_ability_id` int(11) NOT NULL,
  `on_unit_id` int(11) NOT NULL,
  `cc_type` int(11) NOT NULL,
  `cc_max_cooldown` int(11) NOT NULL,
  `cc_current_cooldown` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `game`
--

CREATE TABLE `game` (
  `game_id` int(11) NOT NULL,
  `account_one` int(11) NOT NULL,
  `account_two` int(11) NOT NULL,
  `class_on_turn` int(11) NOT NULL DEFAULT 1,
  `move` int(11) NOT NULL DEFAULT 0,
  `challenge_royal_move` int(11) NOT NULL DEFAULT 30,
  `challenge_royal_activated` int(11) NOT NULL DEFAULT 0,
  `win_account` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `hex`
--

CREATE TABLE `hex` (
  `hex_id` int(11) NOT NULL,
  `game_id` int(11) NOT NULL,
  `hex_column` int(11) NOT NULL,
  `hex_row` int(11) NOT NULL,
  `walkable` int(11) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `modified_hex`
--

CREATE TABLE `modified_hex` (
  `id_modified_hex` int(11) NOT NULL,
  `hex_id` int(11) NOT NULL,
  `ability_id` int(11) NOT NULL,
  `type` int(11) NOT NULL,
  `not_visible_for_class` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `server_ability`
--

CREATE TABLE `server_ability` (
  `id` int(11) NOT NULL,
  `global_id` int(11) NOT NULL,
  `server_unit_id` int(11) NOT NULL,
  `ability_type` int(11) NOT NULL,
  `max_cooldown` int(11) NOT NULL,
  `ability_range` int(11) NOT NULL,
  `quantity` int(11) NOT NULL,
  `cc_time` int(11) NOT NULL,
  `server_ability_name` varchar(20) COLLATE utf8_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Dumping data for table `server_ability`
--

INSERT INTO `server_ability` (`id`, `global_id`, `server_unit_id`, `ability_type`, `max_cooldown`, `ability_range`, `quantity`, `cc_time`, `server_ability_name`) VALUES
(1, 1, 3, 2, 4, 1, 1, 1, 'Earthshake'),
(2, 2, 4, 2, 4, 1, 1, 0, 'Fear'),
(3, 3, 5, 1, 3, 2, 1, 0, 'Joust'),
(4, 4, 6, 1, 3, 2, 1, 1, 'Warstrike'),
(5, 5, 7, 1, 3, 10, 1, 0, 'Powershot'),
(6, 6, 8, 1, 3, 2, 1, 1, 'Trap'),
(7, 7, 9, 0, 0, 0, 0, 0, 'Trics of the trade'),
(8, 8, 10, 0, 0, 1, 1, 0, 'The Fool'),
(9, 9, 11, 1, 2, 2, 1, 0, 'Blessing'),
(10, 10, 11, 1, 4, 2, 1, 1, 'Skyfall'),
(11, 11, 11, 1, 6, 10, 1, 0, 'Fireball'),
(12, 12, 12, 1, 2, 2, 1, 0, 'Necromancy'),
(13, 13, 12, 1, 4, 2, 1, 2, 'Curse'),
(14, 14, 12, 1, 6, 2, 1, 0, 'Vampirism'),
(15, 15, 17, 0, 0, 0, 0, 0, 'Trics of the trade'),
(16, 16, 18, 0, 0, 1, 1, 0, 'The Fool');

-- --------------------------------------------------------

--
-- Table structure for table `server_special_ability`
--

CREATE TABLE `server_special_ability` (
  `id` int(11) NOT NULL,
  `server_unit_type` int(11) NOT NULL,
  `ability_type` int(11) NOT NULL,
  `max_cooldown` int(11) NOT NULL,
  `ability_range` int(11) NOT NULL,
  `quantity` int(11) NOT NULL,
  `cc_time` int(11) NOT NULL,
  `server_ability_name` varchar(20) COLLATE utf8_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Dumping data for table `server_special_ability`
--

INSERT INTO `server_special_ability` (`id`, `server_unit_type`, `ability_type`, `max_cooldown`, `ability_range`, `quantity`, `cc_time`, `server_ability_name`) VALUES
(1, 1, 0, 0, 0, 1, 0, 'Passive Attack'),
(2, 2, 1, 1, 1, 0, 2, 'Push'),
(3, 3, 1, 0, 2, 1, 0, 'Jump'),
(4, 4, 1, 1, 2, 1, 1, 'Trap'),
(5, 5, 1, 0, 2, 2, 0, 'Illusions'),
(6, 6, 1, 0, 2, 0, 0, 'Teleport'),
(7, 7, 1, 0, 3, 2, 0, 'Walk and attack'),
(8, 8, 1, 0, 0, 0, 0, 'Challenge royal'),
(9, 9, 0, 0, 2, 0, 0, 'Illusions');

-- --------------------------------------------------------

--
-- Table structure for table `server_unit`
--

CREATE TABLE `server_unit` (
  `unit_id` int(11) NOT NULL,
  `unit_type` int(1) NOT NULL,
  `unit_class` int(1) NOT NULL,
  `max_health` int(11) NOT NULL,
  `damage` int(11) NOT NULL,
  `attack_range` int(11) NOT NULL,
  `attack_speed` float NOT NULL DEFAULT 0.25
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Dumping data for table `server_unit`
--

INSERT INTO `server_unit` (`unit_id`, `unit_type`, `unit_class`, `max_health`, `damage`, `attack_range`, `attack_speed`) VALUES
(1, 1, 1, 1, 1, 1, 0.25),
(2, 1, 2, 1, 1, 1, 0.25),
(3, 2, 1, 4, 1, 1, 0.25),
(4, 2, 2, 4, 1, 1, 0.25),
(5, 3, 1, 3, 1, 1, 0.25),
(6, 3, 2, 3, 1, 1, 0.25),
(7, 4, 1, 2, 1, 2, 0.25),
(8, 4, 2, 2, 1, 2, 0.25),
(9, 5, 1, 3, 1, 1, 0.25),
(10, 5, 2, 3, 1, 1, 0.25),
(11, 6, 1, 3, 0, 0, 0.25),
(12, 6, 2, 3, 0, 0, 0.25),
(13, 7, 1, 3, 2, 1, 0.25),
(14, 7, 2, 3, 2, 1, 0.25),
(15, 8, 1, 5, 0, 0, 0.25),
(16, 8, 2, 5, 0, 0, 0.25),
(17, 9, 1, 3, 1, 1, 0.25),
(18, 9, 2, 3, 1, 1, 0.25);

-- --------------------------------------------------------

--
-- Table structure for table `unit`
--

CREATE TABLE `unit` (
  `unit_id` int(11) NOT NULL,
  `match_id` int(11) NOT NULL,
  `account_id` int(11) NOT NULL,
  `server_unit_id` int(11) NOT NULL,
  `hex_column` int(11) NOT NULL DEFAULT 0,
  `hex_row` int(11) NOT NULL DEFAULT 0,
  `max_health` int(11) NOT NULL DEFAULT 0,
  `current_health` int(11) NOT NULL DEFAULT 0,
  `damage` int(11) NOT NULL DEFAULT 0,
  `attack_range` int(11) NOT NULL DEFAULT 0,
  `attack_speed` float NOT NULL DEFAULT 0.25,
  `rotation_y` float NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `ability`
--
ALTER TABLE `ability`
  ADD PRIMARY KEY (`ability_id`);

--
-- Indexes for table `accounts`
--
ALTER TABLE `accounts`
  ADD PRIMARY KEY (`account_id`);

--
-- Indexes for table `cc`
--
ALTER TABLE `cc`
  ADD PRIMARY KEY (`cc_id`);

--
-- Indexes for table `game`
--
ALTER TABLE `game`
  ADD PRIMARY KEY (`game_id`);

--
-- Indexes for table `hex`
--
ALTER TABLE `hex`
  ADD PRIMARY KEY (`hex_id`);

--
-- Indexes for table `modified_hex`
--
ALTER TABLE `modified_hex`
  ADD PRIMARY KEY (`id_modified_hex`);

--
-- Indexes for table `server_ability`
--
ALTER TABLE `server_ability`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `server_special_ability`
--
ALTER TABLE `server_special_ability`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `server_unit`
--
ALTER TABLE `server_unit`
  ADD PRIMARY KEY (`unit_id`);

--
-- Indexes for table `unit`
--
ALTER TABLE `unit`
  ADD PRIMARY KEY (`unit_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `ability`
--
ALTER TABLE `ability`
  MODIFY `ability_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `accounts`
--
ALTER TABLE `accounts`
  MODIFY `account_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `cc`
--
ALTER TABLE `cc`
  MODIFY `cc_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `game`
--
ALTER TABLE `game`
  MODIFY `game_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `hex`
--
ALTER TABLE `hex`
  MODIFY `hex_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `modified_hex`
--
ALTER TABLE `modified_hex`
  MODIFY `id_modified_hex` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `server_ability`
--
ALTER TABLE `server_ability`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT for table `server_special_ability`
--
ALTER TABLE `server_special_ability`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `server_unit`
--
ALTER TABLE `server_unit`
  MODIFY `unit_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT for table `unit`
--
ALTER TABLE `unit`
  MODIFY `unit_id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
