<script lang="ts">
	import * as Table from '$lib/components/ui/table';

    const users = [
        {
            username: "alf123teacher",
            role: "Teacher",
        },
        {
            username: "anne123teacher",
            role: "Teacher",
        },
        {
            username: "inger456teacher",
            role: "Teacher",
        },
        {
            username: "jan123teacher",
            role: "Teacher",
        },
        {
            username: "per456teacher",
            role: "Teacher",
        },
        {
            username: "roger123teacher",
            role: "Teacher",
        },
        {
            username: "alexander123elev",
            role: "Student",
        },
        {
            username: "andreas123elev",
            role: "Student",
        },
        {
            username: "emma123elev",
            role: "Student",
        },
        {
            username: "ida456elev",
            role: "Student",
        },
        {
            username: "irene123elev",
            role: "Student",
        },
        {
            username: "jan_elevvgs",
            role: "Student",
        },
        {
            username: "jonas456elev",
            role: "Student",
        },
        {
            username: "julie123elev",
            role: "Student",
        },
        {
            username: "kristian456elev",
            role: "Student",
        },
        {
            username: "markus123elev",
            role: "Student",
        },
        {
            username: "matias123elev",
            role: "Student",
        },
        {
            username: "nora123elev",
            role: "Student",
        },
        {
            username: "sara123elev",
            role: "Student",
        },
        {
            username: "thea456elev",
            role: "Student",
        },
        {
            username: "ulrik123elev",
            role: "Student",
        },
        {
            username: "eva_student",
            role: "Student",
        },
        {
            username: "jakob_student",
            role: "Student",
        },
        {
            username: "kari_student",
            role: "Student",
        },
        {
            username: "kristine789student",
            role: "Student",
        },
        {
            username: "perra789student",
            role: "Student",
        },
        {
            username: "silje789student",
            role: "Student",
        },
        {
            username: "stian789student",
            role: "Student",
        },
        {
            username: "thomas789student",
            role: "Student",
        },
        {
            username: "alf_elevg",
            role: "Student",
        },
        {
            username: "anders_elevvgs",
            role: "Student",
        },
        {
            username: "anita_elevg",
            role: "Student",
        },
        {
            username: "ann_elevg",
            role: "Student",
        },
        {
            username: "asjborn_elevg",
            role: "Student",
        },
        {
            username: "cecilie_elevvgs",
            role: "Student",
        }
    ];
</script>

<div class="w-[720px] mx-auto mt-4 mb-16">
    <h1 class="text-4xl font-semibold my-6">Welcome to Fleximetrics</h1>
    <p>To use the system, sign in with a Feide test user.</p>
    <p>
        Click on <span class="font-bold">Courses (teacher)</span> or <span class="font-bold">Courses (student)</span> to browse the website as a teacher or student.
    </p>

    <h2 class="text-2xl font-semibold mt-6">Feide Test Users</h2>
    <p>Password: <span class="font-bold">098asd</span></p>

    <Table.Root class="mt-2">
        <Table.Header class="border border-black border-2">
            <Table.Row class="border border-gray-600">
                <Table.Head class="font-bold text-black">Username</Table.Head>
                <Table.Head class="font-bold text-black">Role</Table.Head>
            </Table.Row>
        </Table.Header>
        <Table.Body class="border border-black border-2">
            {#each users as user}
                <Table.Row class="border border-gray-600">
                    <Table.Cell>{user.username}</Table.Cell>
                    <Table.Cell>{user.role}</Table.Cell>
                </Table.Row>
            {/each}
        </Table.Body>
    </Table.Root>
</div>

