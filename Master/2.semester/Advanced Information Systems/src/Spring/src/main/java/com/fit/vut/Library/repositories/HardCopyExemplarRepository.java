package com.fit.vut.Library.repositories;

import com.fit.vut.Library.entities.*;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface HardCopyExemplarRepository extends JpaRepository <HardCopyExemplar, Long>  {}

