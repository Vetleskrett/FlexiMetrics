<script lang="ts">
	import { page } from '$app/stores';
	import CreateOrEditCourse from 'src/components/CreateOrEditCourse.svelte';
	import {editCourse, getCourse } from 'src/api';
	import { Semester, type Course } from 'src/types';
	import { goto } from '$app/navigation';
	import { onMount } from 'svelte';

	const courseId = $page.params.courseId;
	let course: Course;
	
	onMount(async () => {
		try {
			course = await getCourse(courseId);
		} catch (error) {
			console.error(error);
		}
	})

	async function editCoursePage(name: string, code: string, year: number, semester: Semester) { 
		try {
			await editCourse(courseId, {
				name: name,
				code: code,
				year: year,
				semester: semester == Semester.Spring ? 0 : 1
			})
			goto(`/teacher/courses/${courseId}`)
		}
		catch(exception){
			console.error("Something Went Wrong!")
	}
	}
</script>

<CreateOrEditCourse 
	courseName={course?.name} 
	courseCode={course?.code}
	year={course?.year}
	semester={course?.semester}
	edit= {true}
	submitFunction={editCoursePage} 
	redirect={`/teacher/courses/${courseId}`}/>
