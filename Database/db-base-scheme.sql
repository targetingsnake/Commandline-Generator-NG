create table mods
(
    id          int               not null,
    name        mediumtext        not null,
    `steam-id`  int               null,
    server_only tinyint default 0 not null,
    client_only tinyint default 0 not null,
    primary key (id),
    constraint `steam-id_UNIQUE`
        unique (`steam-id`)
);

create table modsets
(
    id   int          not null,
    name varchar(150) not null,
    primary key (id)
);

create table mod_mapping
(
    id         int not null,
    modsets_id int not null,
    mods_id    int not null,
    primary key (id),
    constraint fk_mod_mapping_mods1
        foreign key (mods_id) references mods (id),
    constraint fk_mod_mapping_modsets1
        foreign key (modsets_id) references modsets (id)
);

create index fk_mod_mapping_mods1_idx
    on mod_mapping (mods_id);

create index fk_mod_mapping_modsets1_idx
    on mod_mapping (modsets_id);

create table server
(
    id         int          not null,
    name       varchar(150) not null,
    `mod-path` varchar(300) null,
    primary key (id)
);

create table mods_mapping
(
    id           int          not null,
    foldername   varchar(300) null,
    is_installed tinyint      null,
    mods_id      int          not null,
    server_id    int          not null,
    primary key (id),
    constraint fk_mods_folder_mods1
        foreign key (mods_id) references mods (id),
    constraint fk_mods_folder_server1
        foreign key (server_id) references server (id)
);

create index fk_mods_folder_mods1_idx
    on mods_mapping (mods_id);

create index fk_mods_folder_server1_idx
    on mods_mapping (server_id);

create table templates
(
    id         int          not null,
    name       varchar(200) null,
    modsets_id int          not null,
    primary key (id),
    constraint fk_templates_modsets1
        foreign key (modsets_id) references modsets (id)
);

create index fk_templates_modsets1_idx
    on templates (modsets_id);

create definer = root@localhost view servermods as
select `cmdgen_main`.`mods_mapping`.`foldername`   AS `foldername`,
       `cmdgen_main`.`mods_mapping`.`is_installed` AS `is_installed`,
       `m`.`steam-id`                              AS `steam-id`,
       `m`.`name`                                  AS `mod-name`,
       `m`.`server_only`                           AS `server_only`,
       `m`.`client_only`                           AS `client_only`,
       `s`.`name`                                  AS `server-name`,
       `s`.`mod-path`                              AS `mod-path`,
       `s`.`id`                                    AS `server-id`
from ((`cmdgen_main`.`mods_mapping` left join `cmdgen_main`.`mods` `m`
       on (`m`.`id` = `cmdgen_main`.`mods_mapping`.`mods_id`)) left join `cmdgen_main`.`server` `s`
      on (`s`.`id` = `cmdgen_main`.`mods_mapping`.`server_id`));


