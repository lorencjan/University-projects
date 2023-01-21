package com.fit.vut.Library.repositories;

import com.fit.vut.Library.entities.Author;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface AuthorRepository extends JpaRepository <Author, Long> {}
