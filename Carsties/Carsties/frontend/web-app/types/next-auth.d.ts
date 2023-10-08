import { DefaultSession } from 'next-auth'

declare module "next-auth" {
    /**
     * Returned by `useSession`, `getSession` and received as a prop on the `SessionProvider` React Context
     */
    interface Session {
      user: {
        id: string
        username: string
      } & DefaultSession['user']
    }

    interface Profile {
        username: string
    }

    interface User {
      id: string
      username: string
    }
  }

declare module "next-auth/jwt" {
    interface JWT {
        username: string
        access_token?: string
    }
}