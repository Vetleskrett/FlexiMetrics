<script lang="ts">
	import { page } from '$app/stores';
	import AssignmentsCard from 'src/components/AssignmentsCard.svelte';
	import StudentsCard from 'src/components/StudentsCard.svelte';
	import type { Student, Teacher, Team } from 'src/types';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import TeamsCard from 'src/components/TeamsCard.svelte';
	import TeachersCard from 'src/components/TeachersCard.svelte';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';

	const courseId = $page.params.courseId;

	const course = {
		id: '1',
		code: 'TDT101',
		name: 'Programmering',
		year: 2024,
		semester: 'Autumn'
	};

	const assignments = [
		{
			id: '1',
			name: 'Assignment 1',
			due: '18.09.2024',
			individual: false,
			published: true
		},
		{
			id: '2',
			name: 'Assignment 2',
			due: '08.10.2024',
			individual: false,
			published: true
		},
		{
			id: '3',
			name: 'Assignment 3',
			due: '18.10.2024',
			individual: false,
			published: false
		},
		{
			id: '4',
			name: 'Assignment 4',
			due: '08.11.2024',
			individual: false,
			published: false
		}
	];

	const students: Student[] = [];

	for (let i = 1; i <= 196; i++) {
		students.push({
			id: i.toString(),
			email: 'ola@ntnu.no'
		});
	}

	const teams: Team[] = [];
	for (let i = 1; i <= 49; i++) {
		teams.push({
			id: i.toString(),
			students: []
		});
	}

	const teachers: Teacher[] = [
		{
			id: '1',
			email: 'ola@ntnu.no',
			name: 'Ola Nordmann'
		},
		{
			id: '2',
			email: 'kari@ntnu.no',
			name: 'Kari Nordmann'
		}
	];
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>{course.code} - {course.name}</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/knowledge-sharing.png"
				alt="knowledge-sharing"
			/>
			<div>
				<h1 class="ml-4 text-4xl font-semibold">{course.code} - {course.name}</h1>
				<p class="ml-4 font-semibold text-gray-500">{course.year} {course.semester}</p>
			</div>
		</div>

		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				<EllipsisVertical size={32} />
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				<DropdownMenu.Item href="/courses/{courseId}/edit">
					<Pencil class="h-4" />
					<p>Edit course</p>
				</DropdownMenu.Item>
				<DropdownMenu.Item>
					<Trash2 class="h-4" />
					<p>Delete course</p>
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</div>
	<div class="flex flex-row gap-8">
		<div class="flex w-[700px] flex-col gap-8">
			<AssignmentsCard {assignments} {courseId} />
		</div>

		<div class="flex w-[400px] flex-col gap-8">
			<div class="flex flex-row gap-8">
				<StudentsCard {students} {courseId} />
				<TeamsCard {teams} {courseId} />
			</div>
			<TeachersCard {teachers} {courseId} />
		</div>
	</div>
</div>
